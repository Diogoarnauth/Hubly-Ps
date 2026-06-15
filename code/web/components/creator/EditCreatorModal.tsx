'use client';

import React, { useState } from 'react';
import { Button } from '@/components/ui/button';
import usersService from '@/services/api/UsersService';
import creatorService from '@/services/api/CreatorService'; 
import EditCreatorModalProps from '@/services/DTO/creator/EditCreatorModalPropsInputModal';
import { useUser } from '@/providers/UserProvider'; 

export function EditCreatorModal({ 
  currentUsername, 
  currentArtisticName, 
  currentStatus, 
  onClose, 
  onSuccess 
}: EditCreatorModalProps) {
  const { user } = useUser(); 
  const isCoWorker = user?.role === 'coworker'; 

  const [username, setUsername] = useState(currentUsername);
  const [artisticName, setArtisticName] = useState(currentArtisticName); 
  const [status, setStatus] = useState(currentStatus);
  const [loading, setLoading] = useState(false);

  async function handleSave() {
    setLoading(true);
    
    // 🔥 Criamos um array dinâmico de promessas dependendo da role
    const promises: Promise<boolean>[] = [];

    // Se NÃO for coworker, adiciona a edição de username à lista
    if (!isCoWorker) {
      promises.push(usersService.editUsername(username));
    }

    // Estas duas são feitas sempre, independentemente da role
    promises.push(creatorService.editCreator(artisticName));
    promises.push(creatorService.changeStatus(status));

    // Executa apenas os pedidos necessários em paralelo
    const results = await Promise.all(promises);

    if (results.every(res => res === true)) {
      onSuccess();
      onClose();
    } else {
      alert("Failed to save changes. Please try again.");
    }
    setLoading(false);
  }

  return (
    <div className="fixed inset-0 bg-black/80 flex items-center justify-center z-50 p-4">
      <div className="bg-[#2A2A2A] p-8 rounded-[25px] w-full max-w-md space-y-6 border border-zinc-700">
        <h2 className="text-2xl font-bold text-white">Edit Profile</h2>
        
        <div className="space-y-4">
          
          {/* 🔥 Renderização Condicional: Só mostra o campo Username se NÃO for coworker */}
          {!isCoWorker && (
            <div className="space-y-2">
              <label className="text-sm text-zinc-400">Username (Login)</label>
              <input 
                className="w-full bg-[#1A1A1A] border border-zinc-700 p-3 rounded-lg text-white outline-none focus:border-[#A78BFA]"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
              />
            </div>
          )}

          <div className="space-y-2">
            <label className="text-sm text-zinc-400">Artistic Name</label>
            <input 
              className="w-full bg-[#1A1A1A] border border-zinc-700 p-3 rounded-lg text-white outline-none focus:border-[#A78BFA]"
              value={artisticName}
              onChange={(e) => setArtisticName(e.target.value)}
            />
          </div>

          <div className="space-y-2">
            <label className="text-sm text-zinc-400">Availability Status</label>
            <select 
              className="w-full bg-[#1A1A1A] border border-zinc-700 p-3 rounded-lg text-white outline-none focus:border-[#A78BFA]"
              value={status}
              onChange={(e) => setStatus(e.target.value)}
            >
              <option value="AVAILABLE">AVAILABLE</option>
              <option value="UNAVAILABLE">UNAVAILABLE</option>
            </select>
          </div>
        </div>

        <div className="flex gap-4 pt-4">
          <Button variant="ghost" onClick={onClose} className="flex-1 text-white hover:bg-zinc-800">Cancel</Button>
          <Button 
            onClick={handleSave} 
            disabled={loading}
            className="flex-1 bg-[#A78BFA] hover:bg-[#8B5CF6] text-white"
          >
            {loading ? "Saving..." : "Save Changes"}
          </Button>
        </div>
      </div>
    </div>
  );
}