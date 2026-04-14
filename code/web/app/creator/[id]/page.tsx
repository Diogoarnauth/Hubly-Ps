'use client';
import React, { use } from 'react'; 
import { CreatorProfile } from '@/components/creator/CreatorProfile';

export default function CreatorProfilePage({ params }: { params: Promise<{ id: string }> }) {
  const resolvedParams = use(params);
  
  return (
    <div className="min-h-screen bg-black p-8 md:p-10 flex justify-center">
      <div className="w-full max-w-5xl">
        <CreatorProfile id={resolvedParams.id} />
      </div>
    </div>
  );
}