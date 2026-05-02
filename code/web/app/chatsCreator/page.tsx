'use client';
import React, { use } from 'react'; 
import { CreatorChatSelection } from '@/components/chatCreator/CreatorChatSelection';

export default function ChatMessagesPage({ params }: { params: Promise<{ id: string }> }) {
  const resolvedParams = use(params);
  
  return (
    <div className="min-h-screen bg-black p-6 md:p-10 flex justify-center items-start">
      <div className="w-full max-w-5xl mt-10">
        <CreatorChatSelection id={resolvedParams.id} />
      </div>
    </div>
  );
}