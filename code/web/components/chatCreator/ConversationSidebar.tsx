'use client';
import React, { useEffect, useState, useCallback, useRef } from 'react';
import conversationService from '@/services/api/ConversationService';
import ConversationSummary from '@/services/DTO/conversation/ConversationSummaryOutputModel';
import SidebarProps from '@/services/DTO/conversation/SidebarPropsInputModel';
import { useSignalR } from '@/providers/SignalRContext';

export const ConversationSidebar = ({
    profileId,
    onSelectConversation,
    activeConversationId,
    isCompany = false
}: SidebarProps) => {
    const [conversations, setConversations] = useState<ConversationSummary[]>([]);
    const { connection, isConnected } = useSignalR();
    const [loading, setLoading] = useState(true);
    const activeIdRef = useRef(activeConversationId);
    console.log("DEBUG: ConversationSidebar render - activeConversationId:", activeIdRef.current);

    const loadConversations = useCallback(async () => {
        try {
            let response;
            if (isCompany) {
                response = await conversationService.getConversationsByCompanyId(profileId);
            } else {
                response = await conversationService.getConversationsByProfileId(profileId);
            }

            const dataToSet = Array.isArray(response)
                ? response
                : (response?.data || response?.value || []);

            setConversations(dataToSet);
        } catch (err) {
            console.error("Hubly: Erro ao carregar conversas:", err);
            setConversations([]);
        } finally {
            setLoading(false);
        }
    }, [profileId, isCompany]);

    useEffect(() => {
        loadConversations();
    }, [loadConversations]);

    useEffect(() => {
        if (isConnected && connection && profileId) {
            const sidebarTopic = `all_conversations_topic_${profileId}`;

            connection.invoke("JoinTopic", sidebarTopic);

            connection.on("SidebarUpdate", (update: any) => {
                console.log("DEBUG SIDEBAR RECEBIDO:", update);

                if (update.isDeleted || update.type === "MESSAGE_DELETE") {
                    loadConversations();
                } else {
                    setConversations((prev) => {
                        const updated = prev.map((conv) => {
                            if (conv.id === update.conversationId) {

                                if (update.type === "READ_UPDATE") {
                                    console.log("DEBUG READ_UPDATE:", update);
                                    if (update.currentUserId === profileId) {
                                        return { ...conv, unreadCount: 0 };
                                    }
                                    return conv;
                                }

                                if (update.type === "MESSAGE_CREATE") {
                                    console.log("DEBUG MESSAGE_CREATE:", update);
                                    const isFromMe = update.senderId === profileId;

                                    // NOVIDADE: Se a conversa está aberta, o contador deve ser 0
                                    const isChatOpen = activeConversationId === update.conversationId;
                                    console.log("DEBUG: isFromMe:", isFromMe, "isChatOpen:", isChatOpen);

                                    return {
                                        ...conv,
                                        lastMessage: update.content,
                                        lastMessageAt: update.sentAt || new Date().toISOString(),
                                        unreadCount: (isFromMe || isChatOpen) ? 0 : conv.unreadCount + 1
                                    };
                                }

                                if (update.type === "MESSAGE_EDIT") {
                                    console.log("DEBUG MESSAGE_EDIT:", update);
                                    return {
                                        ...conv,
                                        lastMessage: update.content,
                                        lastMessageAt: update.sentAt || new Date().toISOString()
                                    };
                                }
                            }
                            return conv;
                        });

                        return [...updated].sort((a, b) =>
                            new Date(b.lastMessageAt).getTime() - new Date(a.lastMessageAt).getTime()
                        );
                    });
                }
            });

            return () => {
                connection.off("SidebarUpdate");
                connection.invoke("LeaveTopic", sidebarTopic);
            };
        }
    }, [isConnected, connection, profileId, loadConversations]);

    if (loading) return <div className="p-4 text-zinc-500 italic">Loading Messages...</div>;

    return (
        <div className="w-full h-full bg-zinc-900 border-r border-zinc-800 flex flex-col">
            <div className="p-4 border-b border-zinc-800 flex justify-between items-center">
                <h2 className="text-xl font-bold text-white">Messages</h2>
            </div>

            <div className="flex-1 overflow-y-auto custom-scrollbar">
                {conversations.length === 0 ? (
                    <div className="p-4 text-zinc-500 text-sm text-center">No conversations found</div>
                ) : (
                    conversations.map((conv) => (
                        <button
                            key={conv.id}
                            onClick={() => onSelectConversation(conv.id)}
                            className={`w-full p-4 flex items-center gap-3 hover:bg-zinc-800/50 transition-all border-b border-zinc-800/50 ${activeConversationId === conv.id
                                ? 'bg-zinc-800 border-l-4 border-l-blue-500'
                                : 'border-l-4 border-l-transparent'
                                }`}
                        >
                            {/* Avatar */}
                            <div className="w-12 h-12 rounded-full bg-zinc-700 flex-shrink-0 flex items-center justify-center text-white font-bold border border-zinc-600">
                                {conv.otherPartyName ? conv.otherPartyName[0].toUpperCase() : '?'}
                            </div>

                            {/* Conteúdo */}
                            <div className="flex-1 text-left min-w-0">
                                <div className="flex justify-between items-baseline mb-1">
                                    <span className="font-semibold text-white truncate text-sm">
                                        {conv.otherPartyName}
                                    </span>
                                    <div className="flex items-center gap-1 ml-2">
                                        {conv.unreadCount > 0 && (
                                            <span className="bg-blue-500 text-white text-[10px] px-1.5 py-0.5 rounded-full min-w-[16px] text-center">
                                                {conv.unreadCount > 99 ? '99+' : conv.unreadCount}
                                            </span>
                                        )}
                                        <span className="text-[10px] text-zinc-500 whitespace-nowrap">
                                            {new Date(conv.lastMessageAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                                        </span>
                                    </div>
                                </div>
                                <p className="text-xs text-zinc-400 truncate">
                                    {conv.lastMessage || <span className="italic">No messages...</span>}
                                </p>
                            </div>
                        </button>
                    ))
                )}
            </div>
        </div>
    );
};