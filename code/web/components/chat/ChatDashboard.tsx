'use client';
import React, { useState } from 'react';
import { ConversationSidebar } from '../chatCreator/ConversationSidebar';
import { ChatPage } from '@/components/chat/ChatPage';
import { Button } from '@/components/ui/button';
import { ArrowLeft } from 'lucide-react';
import { useRouter } from 'next/navigation';

export const ChatDashboard = ({ profileId, isCompany = false }: { profileId: string, isCompany?: boolean }) => {    const [selectedConvId, setSelectedConvId] = useState<string | null>(null);
    const router = useRouter();

    return (
        <div className="flex flex-col w-full max-w-7xl mx-auto pt-24 px-4 pb-10">
            
            {/* 1. Botão de Voltar */}
            <div className="flex justify-start mb-6 relative z-10">
                <Button
                    variant="ghost"
                    className="text-zinc-400 hover:text-white hover:bg-zinc-800 gap-2 transition-colors py-2 px-4"
                    onClick={() => router.back()}
                >
                    <ArrowLeft className="w-4 h-4" /> Back to Selection
                </Button>
            </div>

            {/* 2. Retângulo Grande do Dashboard */}
            <div className="flex h-[80vh] bg-zinc-900 border border-zinc-800 rounded-3xl overflow-hidden shadow-2xl">
                
                {/* Sidebar (Lista de Mensagens) */}
                <div className="w-1/3 min-w-[300px] border-r border-zinc-800">
                    <ConversationSidebar 
                        profileId={parseInt(profileId)} 
                        isCompany={isCompany}
                        onSelectConversation={(id: number) => setSelectedConvId(id.toString())}
                        activeConversationId={selectedConvId ? parseInt(selectedConvId) : undefined}
                    />
                </div>

                {/* Área do Chat (Mensagens) */}
                <div className="flex-1 bg-zinc-950/50">
                    {selectedConvId ? (
                        <ChatPage id={selectedConvId} />
                    ) : (
                        <div className="h-full flex flex-col items-center justify-center text-zinc-500 space-y-4">
                            <div className="p-4 bg-zinc-900 rounded-full">
                                <span className="text-3xl">💬</span>
                            </div>
                            <p className="text-lg font-medium">Select a conversation to start chatting</p>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};