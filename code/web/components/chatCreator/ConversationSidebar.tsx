'use client';
import React, { useEffect, useState } from 'react';
import conversationService from '@/services/api/ConversationService';
import ConversationSummary from '@/services/DTO/ConversationSummaryOutputModel';

interface SidebarProps {
    profileId: number;
    onSelectConversation: (id: number) => void;
    activeConversationId?: number;
    isCompany?: boolean;
}

export const ConversationSidebar = ({ 
    profileId, 
    onSelectConversation, 
    activeConversationId,
    isCompany = false 
}: SidebarProps) => {
    const [conversations, setConversations] = useState<ConversationSummary[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const load = async () => {
            try {
                let response;
                
                if (isCompany) {
                    console.log("Hubly: Carregando conversas da EMPRESA para ID:", profileId);
                    response = await conversationService.getConversationsByCompanyId(profileId);
                } else {
                    console.log("Hubly: Carregando conversas do CREATOR para ID:", profileId);
                    response = await conversationService.getConversationsByProfileId(profileId);
                }
                
                const dataToSet = Array.isArray(response) 
                    ? response 
                    : (response?.data || response?.value || []);

                setConversations(dataToSet);
            } catch (err) {
                console.error("Error loading conversations:", err);
                setConversations([]); 
            } finally {
                setLoading(false);
            }
        };
        load();
    }, [profileId, isCompany]); 
    

    if (loading) return <div className="p-4 text-zinc-500">Loading conversations...</div>;

    return (
        <div className="w-full h-full bg-zinc-900 border-r border-zinc-800 flex flex-col">
            <div className="p-4 border-b border-zinc-800">
                <h2 className="text-xl font-bold text-white">Messages</h2>
            </div>
            <div className="flex-1 overflow-y-auto">
                    {conversations.map((conv) => (
                        <button
                        key={conv.id}
                        onClick={() => onSelectConversation(conv.id)}
                        className={`w-full p-4 flex items-center gap-3 hover:bg-zinc-800 transition-colors border-b border-zinc-800/50 ${
                            activeConversationId === conv.id ? 'bg-zinc-800 border-l-4 border-l-blue-500' : ''
                        }`}
                    >
                        <div className="w-12 h-12 rounded-full bg-zinc-700 flex-shrink-0 flex items-center justify-center text-white font-bold">
                            {conv.otherPartyName[0]}
                        </div>
                        
                        <div className="flex-1 text-left min-w-0">
                            <div className="flex justify-between items-baseline">
                                <span className="font-semibold text-white truncate">{conv.otherPartyName}</span>
                                <span className="text-[10px] text-zinc-500">
                                    {new Date(conv.lastMessageAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                                </span>
                            </div>
                            <p className="text-sm text-zinc-400 truncate">
                                {conv.lastMessage || "No messages yet"}
                            </p>
                        </div>
                    </button>
                ))}
            </div>
        </div>
    );
};