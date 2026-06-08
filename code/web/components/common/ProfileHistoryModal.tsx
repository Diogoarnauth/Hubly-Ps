'use client';
import React, { useEffect, useState } from 'react';
import { Button } from '@/components/ui/button';
import { Loader2 } from 'lucide-react';
import usersService from '@/services/api/UsersService';
import { ProfileHistoryOutputModel } from '@/services/DTO/ProfileHistoryOutputModel';
import { PagedResponse } from '@/services/DTO/PagedResponse';
import { useRouter } from 'next/navigation';

interface ProfileHistoryModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export default function ProfileHistoryModal({ isOpen, onClose }: ProfileHistoryModalProps) {
  const [historyPage, setHistoryPage] = useState<PagedResponse<ProfileHistoryOutputModel> | null>(null);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(false);
  const router = useRouter();
  const pageSize = 20;

  useEffect(() => {
    if (!isOpen) return;

    setPage(1);
  }, [isOpen]);

  useEffect(() => {
    if (!isOpen) return;

    let mounted = true;
    const load = async (requestedPage: number) => {
      try {
        setLoading(true);
        const data = await usersService.getHistory(requestedPage, pageSize);
        console.log("Profile history data:", data);
        if (!mounted) return;
        setHistoryPage(data);
      } catch (err) {
        console.error('Error loading history:', err);
      } finally {
        if (mounted) setLoading(false);
      }
    };

    load(page);

    return () => { mounted = false; };
  }, [isOpen, page]);

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-[#2A2A2A] p-6 rounded-[20px] w-full max-w-3xl border border-zinc-700 text-white max-h-[90vh] overflow-y-auto shadow-2xl">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-lg font-bold">Profile History</h2>
          <div className="flex items-center gap-2">
            <Button variant="ghost" className="text-zinc-400" onClick={onClose}>Close</Button>
          </div>
        </div>

        <div className="space-y-3">
          {loading && (
            <div className="flex items-center justify-center py-6">
              <Loader2 className="animate-spin w-6 h-6 text-zinc-500" />
            </div>
          )}

          {!loading && (!historyPage || historyPage.items.length === 0) && (
            <p className="text-zinc-500 italic text-center">No history items yet.</p>
          )}

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            {(historyPage?.items ?? []).map(item => (
              <div
                key={item.id}
                className="bg-[#333] p-4 rounded-lg border border-zinc-700 cursor-pointer hover:bg-[#3a3a3a] transition"
                onClick={() => {
                  onClose();
                  if (item.targetType === 'Company') {
                    router.push(`/company/${item.targetId}`);
                  } else {
                    router.push(`/socialProfile/${item.targetId}`);
                  }
                }}
              >
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-zinc-400 uppercase tracking-widest">{item.targetType}</p>
                    <p className="font-medium truncate">{item.targetName}</p>
                  </div>
                  <div className="text-[11px] text-zinc-500">{new Date(item.viewedAt).toLocaleString()}</div>
                </div>
              </div>
            ))}
          </div>

          {!loading && historyPage && historyPage.totalPages > 1 && (
            <div className="flex items-center justify-between mt-4 gap-3">
              <Button
                variant="secondary"
                onClick={() => setPage(Math.max(1, page - 1))}
                disabled={page <= 1}
              >
                Previous
              </Button>
              <p className="text-sm text-zinc-400">
                Page {page} of {historyPage.totalPages}
              </p>
              <Button
                variant="secondary"
                onClick={() => setPage(Math.min(historyPage.totalPages, page + 1))}
                disabled={page >= historyPage.totalPages}
              >
                Next
              </Button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
