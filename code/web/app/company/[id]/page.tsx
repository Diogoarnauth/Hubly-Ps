'use client';
import React, { use } from 'react'; 
import { CompanyProfile } from '@/components/company/CompanyProfile';

export default function CompanyProfilePage({ params }: { params: Promise<{ id: string }> }) {
  const resolvedParams = use(params);
  
  return (
    <div className="min-h-screen bg-black p-8 md:p-10 flex justify-center">
      <div className="w-full max-w-5xl">
        <CompanyProfile id={resolvedParams.id} />
      </div>
    </div>
  );
}