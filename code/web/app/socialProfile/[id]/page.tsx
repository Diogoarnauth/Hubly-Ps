'use client';
import React, { use } from 'react'; 
import { SocialProfile } from '@/components/creator/SocialProfile';

export default function CompanyProfilePage({ params }: { params: Promise<{ id: string }> }) {
  const resolvedParams = use(params);
  const id = resolvedParams.id;

  if (!id || isNaN(Number(id)) || id === 'create') {
    return null; 
  }
  
  return (
    <div className="min-h-screen bg-black p-8 md:p-10 flex justify-center">
      <div className="w-full max-w-5xl">
        
        <SocialProfile profileId={id} />
      </div>
    </div>
  );
}