'use client';

import React, { useEffect, useState, useCallback } from 'react';
import { ArrowLeft, User as UserIcon, Settings, History, Loader2 } from 'lucide-react';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { useRouter } from 'next/navigation';
import usersService, { UserInfo } from '@/services/api/UsersService'; 
import ProfileHistoryModal from '@/components/common/ProfileHistoryModal';
import { toastError } from '../ToastImplementations'; 
import { EditUserModal } from './EditJustUserModal'; 

export function JustUserProfile() {
  const [user, setUser] = useState<UserInfo | null>(null);
  const [loading, setLoading] = useState(true);
  const [isHistoryModalOpen, setIsHistoryModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const router = useRouter();

  const loadUserData = useCallback(async () => {
    try {
      setLoading(true);
      const myInfo = await usersService.getCurrentUser();
      
      if (myInfo) {
        setUser(myInfo);
      } else {
        toastError('Error', 'Failed to load session data');
        router.push('/login');
      }
    } catch (error) {
      console.error("Error loading user info via service:", error);
      toastError('Error', 'An error occurred while fetching your profile');
      router.push('/');
    } finally {
      setLoading(false);
    }
  }, [router]);

  // 🔥 2. O useEffect limita-se a chamar a função quando o componente monta
  useEffect(() => {
    loadUserData();
  }, [loadUserData]);

  if (loading) {
    return (
      <div className="flex min-h-[400px] items-center justify-center text-white">
        <Loader2 className="w-8 h-8 animate-spin text-zinc-400" />
      </div>
    );
  }

  return (
    <div className="text-white relative pt-[5vh]">
      
      {/* Botões de Ações de Conta (Dono do Perfil) */}
      <div className="flex justify-end items-center mb-4 gap-3">
        <Button
          variant="ghost"
          size="icon"
          className="hover:bg-zinc-800"
          onClick={() => setIsHistoryModalOpen(true)}
          title="View History"
        >
          <History className="w-8 h-8 text-white" />
        </Button>

        <Button
          variant="ghost"
          size="icon"
          className="hover:bg-zinc-800"
          title="Account Settings"
          onClick={() => setIsEditModalOpen(true)}
        >
          <Settings className="w-8 h-8 text-white" />
        </Button>
      </div>

      {/* Botão de Voltar */}
      <div className="flex justify-start mb-4">
        <Button
          variant="ghost"
          className="text-zinc-400 hover:text-white hover:bg-zinc-800 gap-2"
          onClick={() => router.back()}
        >
          <ArrowLeft className="w-4 h-4" /> Back
        </Button>
      </div>

      {/* Cabeçalho do Perfil */}
      <div className="flex flex-col items-center mb-12">
        <div className="w-32 h-32 bg-zinc-800 rounded-full flex items-center justify-center mb-3">
          <UserIcon className="w-16 h-16 text-zinc-400" />
        </div>
        <h1 className="text-2xl font-semibold">My Account</h1>
        <p className="text-sm text-zinc-400 mt-1">Standard Hubly User</p>
        <div className="w-full max-w-5xl opacity-50 h-[1px] bg-zinc-500 mt-2"></div>
      </div>

      {/* Conteúdo Centralizado */}
      <div className="max-w-2xl mx-auto">
        <Card className="bg-[#414141] border-none text-white rounded-[25px]">
          <CardContent className="p-8 space-y-4">
            <h3 className="text-zinc-400 font-bold uppercase text-[10px] tracking-widest mb-4">
              Account Information
            </h3>
            
            <div className="space-y-3">
              <p className="text-xl font-light">
                <span className="font-bold">Name:</span> {user?.name || "N/A"}
              </p>
              <p className="text-xl font-light">
                <span className="font-bold">Email:</span> {user?.email || "N/A"}
              </p>
              <p className="text-xl font-light">
                <span className="font-bold">Account Type:</span> {user?.role || "Standard User"}
              </p>
            </div>

            {/* Caixa de aviso amigável a explicar o estado da conta */}
            <div className="mt-8 p-4 bg-zinc-800/50 rounded-xl border border-zinc-700/50 text-sm text-zinc-300">
              <p>
                💡 <strong>Awaiting action:</strong> You currently have a basic account. To access all features of Hubly, you can create your professional profile (either creator or company profile) or wait for a team invitation (Coworker).
              </p>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* */}
      {isEditModalOpen && (
        <EditUserModal
          currentUsername={user?.name || ''}
          onClose={() => setIsEditModalOpen(false)}
          onSuccess={async () => {
            setIsEditModalOpen(false); 
            await loadUserData();
          }}
        />
      )}

      {/* Modal do Histórico */}
      <ProfileHistoryModal isOpen={isHistoryModalOpen} onClose={() => setIsHistoryModalOpen(false)} />
    </div>
  );
}