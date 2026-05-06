'use client';
import React, { useEffect, useState, useRef } from 'react';
import { useSignalR } from '@/providers/SignalRContext';
import conversationService from '@/services/api/ConversationService';
import userService from '@/services/api/UsersService';
import Message from '@/services/DTO/message/MessageOutputModel';
import ChatPageProps from '@/services/DTO/conversation/ChatPagePropsOutputModel';
import { MoreVertical, Pencil, Trash2, X, Check } from 'lucide-react';

interface ChatPageProps {
  id: string;
}

export const ChatPage = ({ id }: ChatPageProps) => {
  const { connection, isConnected } = useSignalR();
  const [messages, setMessages] = useState<Message[]>([]);
  const [inputValue, setInputValue] = useState('');
  const scrollRef = useRef<HTMLDivElement>(null);
  const containerRef = useRef<HTMLDivElement>(null);
  const [currentUserId, setCurrentUserId] = useState<number | null>(null);

  const [page, setPage] = useState(1);
  const [pageSize] = useState(5);
  const [hasMore, setHasMore] = useState(true);
  const [isLoading, setIsLoading] = useState(false);

  const [activeMenuId, setActiveMenuId] = useState<number | null>(null);
  const [editingMessageId, setEditingMessageId] = useState<number | null>(null);
  const [editValue, setEditValue] = useState('');

  useEffect(() => {
    const fetchMe = async () => {
        const myProfile = await userService.getCurrentUser(); 
        if (myProfile) {
          setCurrentUserId(myProfile.id);
        }
    };
    fetchMe();
  }, []);

  const loadMessages = async (pageNum: number) => {
    if (isLoading) return;
    setIsLoading(true);

    try {
      const data = await conversationService.getMessages(parseInt(id), pageNum, pageSize);
      if (data && data.items) {
        const newMessages = data.items.toReversed();

        setMessages((prev) => {
          if (pageNum === 1) return newMessages;
          return [...newMessages, ...prev];
        });

        // Verifica se chegámos ao fim das mensagens
        if (data.items.length < pageSize) {
          setHasMore(false);
        }
      }
    } catch (error) {
      console.error("Erro ao carregar mensagens:", error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    setMessages([]);
    setPage(1);
    setHasMore(true);
    loadMessages(1);
  }, [id]);

  // Mark messages as read when messages are loaded
  useEffect(() => {
    console.log("BHHHHHHHHH");
    if (messages.length > 0 && currentUserId) {
      const lastMessage = messages[messages.length - 1];
      const lastMessageId = Number(lastMessage.id);
      if (!Number.isInteger(lastMessageId)) {
        console.warn("Invalid message id for mark-read:", lastMessage.id);
        return;
      }

      conversationService.markMessagesAsRead(parseInt(id), lastMessageId);
    }
  }, [messages, currentUserId, id]);

  const handleShowMore = () => {
    const nextPage = page + 1;
    setPage(nextPage);
    loadMessages(nextPage);
  };

  useEffect(() => {
    console.log("🟢 ESTADO ATUALIZA q DO (messages):", messages);
  }, [messages]);

  // SignalR
  useEffect(() => {
    if (isConnected && connection) {
      connection.invoke("JoinTopic", `chat_${id}`);

      connection.on("NewMessage", (newMessage: Message) => {
        console.log("AHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH: messages", messages);
        setMessages((prev) => {
          if (prev.some(m => m.messageId === newMessage.messageId)) return prev;
          return [...prev, newMessage];
        });
      });

      connection.on("MessageUpdated", (updatedMsg: any) => {
        setMessages((prev) => {
          if (updatedMsg.isDeleted) {
            return prev.filter(m => m.id !== updatedMsg.id);
          }
          return prev.map(m =>
            m.id === updatedMsg.id
              ? { ...m, content: updatedMsg.content, isEdited: updatedMsg.isEdited }
              : m
          );
        });
      });

      return () => {
        connection.off("NewMessage");
        connection.off("MessageUpdated");
        connection.invoke("LeaveTopic", `chat_${id}`);
      };
    }
  }, [isConnected, connection, id]);

  useEffect(() => {
    if (page === 1) {
      scrollRef.current?.scrollIntoView({ behavior: 'smooth' });
    }
  }, [messages, page]);

  const formatMessageTime = (timestamp: number) => {
    const now = new Date();
    const msgDate = new Date(timestamp * 1000); // Assuming timestamp is in seconds
    const isToday = msgDate.toDateString() === now.toDateString();
    
    if (isToday) {
      return msgDate.toLocaleTimeString('pt-PT', { hour: '2-digit', minute: '2-digit' });
    } else {
      return msgDate.toLocaleDateString('pt-PT', { day: '2-digit', month: '2-digit' }) + ' ' + 
             msgDate.toLocaleTimeString('pt-PT', { hour: '2-digit', minute: '2-digit' });
    }
  };

  const handleSendMessage = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!inputValue.trim()) return;

    const content = inputValue;
    setInputValue('');

    const result = await conversationService.sendMessage(parseInt(id), content);
    if (!result.success) {
      console.error(result.message);
    }
  };

  const handleDelete = async (msgId: number) => {
    if (!confirm("Delete this message?")) return;
    const res = await conversationService.deleteMessage(msgId);
    if (res) {
      setMessages(prev => prev.filter(m => m.id !== msgId));
      setActiveMenuId(null);
    }
  };

  const handleEditInit = (msg: Message) => {
    setEditingMessageId(msg.id);
    setEditValue(msg.content);
    setActiveMenuId(null);
  };

  const handleUpdateMessage = async (msgId: number) => {
    if (!editValue.trim()) return;
    const res = await conversationService.editMessage(msgId, editValue);
    if (res) {
      setMessages(prev => prev.map(m => m.id === msgId ? { ...m, content: editValue, isEdited: true } : m));
      setEditingMessageId(null);
    }
  };

  return (
    <div className="flex flex-col h-[70vh] bg-zinc-900 border border-zinc-800 rounded-2xl overflow-hidden shadow-2xl text-white">
      <div className="p-4 border-b border-zinc-800 bg-zinc-900/50 backdrop-blur-md flex justify-between items-center">
        <div>
          <h3 className="font-semibold">Conversation #{id}</h3>
          <p className="text-[10px] uppercase tracking-wider text-zinc-500">
            {isConnected ? "● Online" : "○ Connecting..."}
          </p>
        </div>
      </div>

      <div ref={containerRef} className="flex-1 overflow-y-auto p-4 space-y-4">

        {/* Botão de Carregar Mais */}
        {hasMore && (
          <div className="flex justify-center pb-2">
            <button
              onClick={handleShowMore}
              disabled={isLoading}
              className="text-xs bg-zinc-800 hover:bg-zinc-700 text-zinc-400 px-3 py-1 rounded-full border border-zinc-700 transition-colors"
            >
              {isLoading ? "Loading..." : "Show more messages"}
            </button>
          </div>
        )}

        {messages.map((msg) => {
          const isMine = msg.senderId === currentUserId;
          const isEditing = editingMessageId === msg.id;

          return (
            <div key={msg.id} className={`flex group ${isMine ? 'justify-end' : 'justify-start'}`}>
              <div className={`relative max-w-[80%] flex flex-col ${isMine ? 'items-end' : 'items-start'}`}>

                {/* Balão de Mensagem */}
                <div className={`px-4 py-2 rounded-2xl text-sm transition-all relative ${isMine ? 'bg-blue-600 text-white rounded-tr-none' : 'bg-zinc-800 text-zinc-200 rounded-tl-none border border-zinc-700'
                  }`}>

                  {isEditing ? (
                    <div className="flex flex-col gap-2 min-w-[200px]">
                      <input
                        value={editValue}
                        onChange={(e) => setEditValue(e.target.value)}
                        className="bg-zinc-900 border border-zinc-700 rounded p-1 text-white outline-none"
                        autoFocus
                      />
                      <div className="flex justify-end gap-2">
                        <button onClick={() => setEditingMessageId(null)} className="text-zinc-400 hover:text-white"><X size={14} /></button>
                        <button onClick={() => handleUpdateMessage(msg.id)} className="text-green-400 hover:text-green-300"><Check size={14} /></button>
                      </div>
                    </div>
                  ) : (
                    <>
                      <p>{msg.content}</p>
                      {msg.isEdited && <span className="text-[9px] opacity-50 block mt-1">(edited)</span>}
                    </>
                  )}

                  {/* Botão Três Pontinhos (Apenas para o Dono) */}
                  {isMine && !isEditing && (
                    <button
                      onClick={() => setActiveMenuId(activeMenuId === msg.id ? null : msg.id)}
                      className="absolute -left-8 top-1/2 -translate-y-1/2 opacity-0 group-hover:opacity-100 p-1 hover:bg-zinc-800 rounded-full transition-all text-zinc-500"
                    >
                      <MoreVertical size={16} />
                    </button>
                  )}

                  {/* Dropdown Menu */}
                  {activeMenuId === msg.id && (
                    <div className="absolute right-0 top-10 z-50 bg-zinc-800 border border-zinc-700 rounded-lg shadow-xl py-1 min-w-[120px] overflow-hidden">
                      <button onClick={() => handleEditInit(msg)} className="w-full flex items-center gap-2 px-3 py-2 text-xs hover:bg-zinc-700 transition-colors text-zinc-200">
                        <Pencil size={12} /> Edit Message
                      </button>
                      <button onClick={() => handleDelete(msg.id)} className="w-full flex items-center gap-2 px-3 py-2 text-xs hover:bg-red-900/30 text-red-400 hover:bg-zinc-700 transition-colors">
                        <Trash2 size={12} /> Delete Message
                      </button>
                    </div>
                  )}
                </div>
              </div>
            </div>
          );
        })}
        <div ref={scrollRef} />
      </div>

      <form onSubmit={handleSendMessage} className="p-4 bg-zinc-900 border-t border-zinc-800">
        <div className="flex gap-2">
          <input
            type="text"
            value={inputValue}
            onChange={(e) => setInputValue(e.target.value)}
            placeholder="Write a message..."
            className="flex-1 bg-zinc-800 border border-zinc-700 text-white rounded-xl px-4 py-2 focus:ring-2 focus:ring-blue-500 outline-none"
          />
          <button
            type="submit"
            disabled={!isConnected}
            className="bg-blue-600 hover:bg-blue-500 disabled:opacity-50 text-white px-6 py-2 rounded-xl font-medium transition-all"
          >
            Send
          </button>
        </div>
      </form>
    </div>
  );
};