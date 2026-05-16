'use client';
import React, { useEffect, useState, useCallback, useRef } from 'react';
import { Plus, Tag as TagIcon } from 'lucide-react';
import conversationService from '@/services/api/ConversationService';
import conversationTagService from '@/services/api/ConversationTagService';
import creatorService from '@/services/api/CreatorService';
import ConversationSummary from '@/services/DTO/conversation/ConversationSummaryOutputModel';
import { GetSocialProfileOutputModel } from '@/services/DTO/creator/GetSocialProfileOutputModel';
import SidebarProps from '@/services/DTO/conversation/SidebarPropsInputModel';
import { useSignalR } from '@/providers/SignalRContext';
import { CreateTagModal } from './CreateTagModal';



const TagBadge = ({ tag }: { tag: any }) => (
    <div 
        className="absolute -left-1 top-1/2 -translate-y-1/2 z-10 flex items-center"
        title={tag.tagName}
    >
        <span
            className="px-2 py-1 rounded-r-md text-[10px] font-bold uppercase tracking-tighter shadow-md whitespace-nowrap"
            style={{
                backgroundColor: tag.colorHex || '#3b82f6',
                color: '#fff',
                borderLeft: '2px solid rgba(0,0,0,0.2)'
            }}
        >
            {tag.tagName}
        </span>
    </div>
);


export const ConversationSidebar = ({
    profileId,
    onSelectConversation,
    activeConversationId,
    isCompany = false
}: SidebarProps) => {
    const [conversations, setConversations] = useState<ConversationSummary[]>([]);
    const [availableTags, setAvailableTags] = useState<any[]>([]);
    const [creatorProfile, setCreatorProfile] = useState<GetSocialProfileOutputModel | null>(null);
    const [activeMenuId, setActiveMenuId] = useState<string | number | null>(null);
    const { connection, isConnected } = useSignalR();
    const [loading, setLoading] = useState(true);
    const menuRef = useRef<HTMLDivElement>(null);
    const [isCreateTagModalOpen, setIsCreateTagModalOpen] = useState(false);
    const [isTagCreating, setIsTagCreating] = useState(false);
    const [pendingConversationId, setPendingConversationId] = useState<string | number | null>(null);

   useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
                setActiveMenuId(null);
            }
        };
        document.addEventListener("mousedown", handleClickOutside);
        return () => document.removeEventListener("mousedown", handleClickOutside);
    }, []);

    const loadTags = useCallback(async () => {
        const tags = await conversationTagService.getUserTags();
        setAvailableTags(tags);
    }, []);

    const handleAssignExistingTag = async (conversationId: string | number, tagId: number) => {
        const success = await conversationTagService.tagConversation(Number(conversationId), tagId);
        if (success) {
            setActiveMenuId(null);
            loadConversations();
        }
    };

    const handleCreateAndAssignTag = async (conversationId: string | number) => {
        setPendingConversationId(conversationId);
        setIsCreateTagModalOpen(true);
    };

    const handleCreateTagConfirm = async (tagName: string, colorHex: string) => {
        if (!pendingConversationId) return;

        const tagData = {
            conversationId: pendingConversationId,
            tagName: tagName,
            colorHex: colorHex
        };

        try {
            setIsTagCreating(true);
            const result = await conversationTagService.createTag(tagData);
            if (result.success) {
                // Após criar a tag global, precisamos associá-la a esta conversa específica
                await conversationTagService.tagConversation(Number(pendingConversationId), result.data.id);
                loadTags();
                loadConversations();
                setActiveMenuId(null);
            }
        } catch (error) {
            console.error("Hubly: Error creating tag", error);
        } finally {
            setIsTagCreating(false);
            setIsCreateTagModalOpen(false);
            setPendingConversationId(null);
        }
    };

    const loadConversations = useCallback(async () => {
        try {
            let response;
            if (isCompany) {
                response = await conversationService.getConversationsByCompanyId(profileId);
            } else {
                response = await conversationService.getConversationsByProfileId(profileId);
            }
            const dataToSet = Array.isArray(response) ? response : (response?.data || []);
            setConversations(dataToSet);
        } catch (err) {
            setConversations([]);
        } finally {
            setLoading(false);
        }
    }, [profileId, isCompany]);

    useEffect(() => {
        loadConversations();
        loadTags();
    }, [loadConversations, loadTags]);

    useEffect(() => {
        if (!isCompany && profileId) {
            const loadCreatorProfile = async () => {
                try {
                    const profile = await creatorService.getSocialProfileById(profileId);
                    setCreatorProfile(profile);
                } catch (error) {
                    console.error("Hubly: Error loading creator profile:", error);
                    setCreatorProfile(null);
                }
            };

            loadCreatorProfile();
        }
    }, [isCompany, profileId]);

    useEffect(() => {
        if (creatorProfile) {
            console.log("Creator profile loaded:", creatorProfile);
        }
    }, [creatorProfile]);

    useEffect(() => {
        if (isConnected && connection && profileId) {
            const sidebarTopic = `all_conversations_topic_${profileId}`;

            connection.invoke("JoinTopic", sidebarTopic);

            connection.on("SidebarUpdate", (update: any) => {

                if (update.isDeleted || update.type === "MESSAGE_DELETE") {
                    loadConversations();
                } else {
                    setConversations((prev) => {
                        const updated = prev.map((conv) => {
                            if (conv.id === update.conversationId) {
                                const currentUserId = isCompany ? profileId : creatorProfile?.creatorId;

                                if (update.type === "READ_UPDATE") {
                                    if (update.currentUserId === currentUserId) {
                                        return { ...conv, unreadCount: 0 };
                                    }
                                    return conv;
                                }

                                if (update.type === "MESSAGE_CREATE") {
                                    const isFromMe = update.senderId === currentUserId;
                                    return {
                                        ...conv,
                                        lastMessage: update.content,
                                        lastMessageAt: update.sentAt || new Date().toISOString(),
                                        unreadCount: (isFromMe) ? 0 : conv.unreadCount + 1
                                    };
                                }

                                if (update.type === "MESSAGE_EDIT") {
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
        <>
            <CreateTagModal 
                isOpen={isCreateTagModalOpen}
                onClose={() => {
                    setIsCreateTagModalOpen(false);
                    setPendingConversationId(null);
                }}
                onConfirm={handleCreateTagConfirm}
                isLoading={isTagCreating}
            />
            <div className="w-full h-full bg-zinc-900 border-r border-zinc-800 flex flex-col relative">
            <div className="p-4 border-b border-zinc-800">
                <h2 className="text-xl font-bold text-white">Messages</h2>
            </div>

            <div className="flex-1 overflow-y-auto custom-scrollbar">
                {conversations.map((conv) => (
                    <div
                        key={conv.id}
                        onClick={() => onSelectConversation(conv.id)}
                        className={`w-full p-4 flex items-center gap-3 hover:bg-zinc-800/40 transition-all border-b border-zinc-800/50 cursor-pointer relative group ${
                            activeConversationId === conv.id ? 'bg-zinc-800' : ''
                        }`}
                    >
                        {/* A "Etiqueta" Lateral */}
                        {conv.tag && <TagBadge tag={conv.tag} />}

                        <div className="w-12 h-12 rounded-full bg-zinc-700 flex-shrink-0 flex items-center justify-center text-white font-bold border border-zinc-600 relative">
                            {conv.otherPartyName ? conv.otherPartyName[0].toUpperCase() : '?'}
                        </div>

                        <div className="flex-1 min-w-0">
                            <div className="flex justify-between items-center mb-1">
                                <span className="font-semibold text-white truncate text-sm">
                                    {conv.otherPartyName}
                                </span>
                                <div className="flex items-center gap-2">
                                    {/* Botão de Gestão de Tags (Aparece no Hover ou se menu aberto) */}
                                    <div className="relative" ref={activeMenuId === conv.id ? menuRef : null}>
                                        <button
                                            onClick={(e) => {
                                                e.stopPropagation();
                                                setActiveMenuId(activeMenuId === conv.id ? null : conv.id);
                                            }}
                                            className="opacity-0 group-hover:opacity-100 p-1 hover:bg-zinc-700 rounded transition-opacity"
                                        >
                                            <TagIcon size={14} className="text-zinc-400" />
                                        </button>

                                        {/* Dropdown de Tags */}
                                        {activeMenuId === conv.id && (
                                            <div className="absolute right-0 mt-2 w-48 bg-zinc-800 border border-zinc-700 rounded-md shadow-xl z-50 py-1 overflow-hidden">
                                                <p className="px-3 py-1.5 text-[10px] font-bold text-zinc-500 uppercase border-b border-zinc-700">Existing Tags</p>
                                                <div className="max-h-40 overflow-y-auto">
                                                    {availableTags.map(tag => (
                                                        <button
                                                            key={tag.id}
                                                            onClick={(e) => {
                                                                e.stopPropagation();
                                                                handleAssignExistingTag(conv.id, tag.id);
                                                            }}
                                                            className="w-full text-left px-3 py-2 text-xs text-zinc-300 hover:bg-zinc-700 flex items-center gap-2"
                                                        >
                                                            <div className="w-2 h-2 rounded-full" style={{ backgroundColor: tag.colorHex }} />
                                                            {tag.tagName}
                                                        </button>
                                                    ))}
                                                </div>
                                                <button
                                                    onClick={(e) => {
                                                        e.stopPropagation();
                                                        handleCreateAndAssignTag(conv.id);
                                                    }}
                                                    className="w-full text-left px-3 py-2 text-xs text-blue-400 hover:bg-blue-500/10 border-t border-zinc-700 flex items-center gap-2"
                                                >
                                                    <Plus size={12} />
                                                    Create New Tag
                                                </button>
                                            </div>
                                        )}
                                    </div>
                                    <span className="text-[10px] text-zinc-500 whitespace-nowrap">
                                        {new Date(conv.lastMessageAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                                    </span>
                                </div>
                            </div>
                            <p className="text-xs text-zinc-400 truncate">
                                {conv.lastMessage || <span className="italic text-zinc-600">No messages yet...</span>}
                            </p>
                        </div>
                    </div>
                ))}
            </div>
            </div>
        </>
    );
};