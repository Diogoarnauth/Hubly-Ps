'use client';

import React, { useState } from 'react';
import { Loader2, X } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { toastError, toastSuccess } from '../ToastImplementations';
import usersService from '@/services/api/UsersService';

interface EditUserModalProps {
  currentUsername: string;
  onClose: () => void;
  onSuccess: () => void;
}

export function EditUserModal({ currentUsername, onClose, onSuccess }: EditUserModalProps) {
  const [username, setUsername] = useState(currentUsername);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!username.trim()) {
      toastError('Error', 'Username cannot be empty');
      return;
    }

    try {
      setIsSubmitting(true);
      // Chamas a função do teu service que faz o Post para o backend
      const success = await usersService.editUsername(username);

      if (success) {
        toastSuccess('Success', 'Profile updated successfully!');
        onSuccess();
        onClose();
      } else {
        toastError('Error', 'Failed to update username');
      }
    } catch (error) {
      console.error('Error updating username:', error);
      toastError('Error', 'An unexpected error occurred');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4">
      <div className="bg-[#2a2a2a] w-full max-w-md rounded-[25px] border border-zinc-800 text-white overflow-hidden shadow-2xl animate-in fade-in zoom-in-95 duration-150">
        
        {/* Header do Modal */}
        <div className="p-6 flex items-center justify-between border-b border-zinc-800">
          <h2 className="text-xl font-semibold">Edit Profile</h2>
          <Button variant="ghost" size="icon" className="hover:bg-zinc-800 text-zinc-400 hover:text-white" onClick={onClose}>
            <X className="w-5 h-5" />
          </Button>
        </div>

        {/* Formulário */}
        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          <div className="space-y-2">
            <label className="text-xs font-bold uppercase tracking-wider text-zinc-400">
              Name / Username
            </label>
            <input
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              className="w-full bg-zinc-800 border border-zinc-700 rounded-xl px-4 py-3 text-sm text-white focus:outline-none focus:border-zinc-500 transition-colors"
              placeholder="Enter your name"
              disabled={isSubmitting}
            />
          </div>

          {/* Botões de Ação */}
          <div className="flex justify-end gap-3 pt-4 border-t border-zinc-800 mt-6">
            <Button
              type="button"
              variant="ghost"
              className="hover:bg-zinc-800 text-zinc-400 hover:text-white"
              onClick={onClose}
              disabled={isSubmitting}
            >
              Cancel
            </Button>
            <Button
              type="submit"
              className="bg-white hover:bg-zinc-200 text-black font-semibold px-5"
              disabled={isSubmitting}
            >
              {isSubmitting ? (
                <>
                  <Loader2 className="w-4 h-4 animate-spin mr-2" />
                  Saving...
                </>
              ) : (
                'Save Changes'
              )}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}