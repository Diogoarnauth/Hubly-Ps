'use client';
import React, { use } from 'react'; 
import { ChatDashboard } from '@/components/chat/ChatDashboard';

export default function ChatMessagesPage({ params }: { params: Promise<{ id: string }> }) {
  const resolvedParams = use(params);
  
  return (
    <div className="min-h-screen bg-black p-8 md:p-10 flex justify-center items-start">
      <div className="w-full max-w-7xl">
        <ChatDashboard profileId={resolvedParams.id} />
      </div>
    </div>
  );
}