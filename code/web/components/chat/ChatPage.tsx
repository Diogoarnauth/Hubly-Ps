'use client';
import React, { useEffect, useState, useRef } from 'react';
import { useSignalR } from '@/providers/SignalRContext'; 
import conversationService from '@/services/api/ConversationService';
import userService from '@/services/api/UsersService';


interface ChatPageProps {
  id: string;
}

interface Message {
    content: string;
    conversationId: number;
    id: number;
    isEdited: boolean;
    senderId: number;
    sentAt: number;
}

export const ChatPage = ({ id }: ChatPageProps) => {
  const { connection, isConnected } = useSignalR();
  const [messages, setMessages] = useState<Message[]>([]);
  const [inputValue, setInputValue] = useState('');
  const scrollRef = useRef<HTMLDivElement>(null);
  const [currentUserId, setCurrentUserId] = useState<number | null>(null);


  useEffect(() => {
    const fetchMe = async () => {
        const myProfile = await userService.getCurrentUser(); 
        setCurrentUserId(myProfile.id);
    };
    fetchMe();
}, []);

  // 1. history
  useEffect(() => {
    console.log("Loading conversation history for conversationId:", id);
    const loadHistory = async () => {
      const data = await conversationService.getMessages(parseInt(id));
      if (data) {
        setMessages(data.items.toReversed());
      }
    };
    loadHistory();
  }, [id]);

  // 2. SignalR
  useEffect(() => {
    if (isConnected && connection) {
      connection.invoke("JoinTopic", `chat_${id}`);

      connection.on("NewMessage", (newMessage: Message) => {
        console.log("Nova mensagem recebida via SignalR:", newMessage);
        setMessages((prev) => {
          if (prev.some(m => m.id === newMessage.id)) return prev;
          return [...prev, newMessage];
        });
      });

      return () => {
        connection.off("NewMessage");
        connection.invoke("LeaveTopic", `chat_${id}`);
      };
    }
  }, [isConnected, connection, id]);


  useEffect(() => {
    scrollRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

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

  return (
    <div className="flex flex-col h-[70vh] bg-zinc-900 border border-zinc-800 rounded-2xl overflow-hidden shadow-2xl text-white">
      <div className="p-4 border-b border-zinc-800 bg-zinc-900/50 backdrop-blur-md flex justify-between items-center">
        <div>
          <h3 className="font-semibold">Conversation #{id}</h3>
          <p className="text-[10px] uppercase tracking-wider text-zinc-500">
            {isConnected ? "● Online" : "○ Conecting..."}
          </p>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto p-4 space-y-4">
        {messages.map((msg) => (
          <div 
            key={msg.id} 
            className={`flex ${msg.senderId === currentUserId ? 'justify-end' : 'justify-start'}`}
          >
            <div className={`max-w-[75%] px-4 py-2 rounded-2xl text-sm shadow-sm ${
              msg.senderId === currentUserId 
                ? 'bg-blue-600 text-white rounded-tr-none' 
                : 'bg-zinc-800 text-zinc-200 rounded-tl-none border border-zinc-700'
            }`}>
              {msg.content}
            </div>
          </div>
        ))}
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