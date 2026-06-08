'use client';
import React, { useEffect, useState } from 'react';
import { Button } from '@/components/ui/button';
import { Loader2 } from 'lucide-react';
import usersService from '@/services/api/UsersService';
import { ProfileHistoryOutputModel } from '@/services/DTO/ProfileHistoryOutputModel';
import { useRouter } from 'next/navigation';

interface ProfileHistoryModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export default function ProfileHistoryModal({ isOpen, onClose }: ProfileHistoryModalProps) {
  const [history, setHistory] = useState<ProfileHistoryOutputModel[]>([]);
  const [loading, setLoading] = useState(false);
  const router = useRouter();

  useEffect(() => {
    if (!isOpen) return;

    let mounted = true;
    const load = async () => {
      try {
        setLoading(true);
        const data = await usersService.getHistory();
        console.log("Profile history data:", data);
        if (!mounted) return;
        setHistory(data || []);
      } catch (err) {
        console.error('Error loading history:', err);
      } finally {
        if (mounted) setLoading(false);
      }
    };

    load();

    return () => { mounted = false; };
  }, [isOpen]);

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

          {!loading && history.length === 0 && (
            <p className="text-zinc-500 italic text-center">No history items yet.</p>
          )}

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            {history.map(item => (
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
        </div>
      </div>
    </div>
  );
}
