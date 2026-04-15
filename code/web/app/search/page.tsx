'use client';

import { useState } from 'react';
import { RoleSelectionSearch } from './_components/RoleSelectionSearch';
import { CreatorSearch } from '@/components/search/CreatorSearch';
import { CompanySearch } from '@/components/search/CompanySearch'; 

export default function SearchPage() {
  const [searchMode, setSearchMode] = useState<'creator' | 'company' | null>(null);

  return (
    <div className="flex min-h-[80vh] w-full items-center justify-center p-6 md:p-10">
      <div className="w-full max-w-6xl">
        
        {/* Seleção Inicial (Igual ao Onboarding mas com texto de Search) */}
        {!searchMode && (
          <RoleSelectionSearch onSelect={(mode) => setSearchMode(mode)} />
        )}
        
        {/* Motor de Pesquisa de Creators */}
        {searchMode === 'creator' && (
          <div className="animate-in fade-in slide-in-from-bottom-4 duration-500">
             <CreatorSearch onBack={() => setSearchMode(null)} />
          </div>
        )}

        {/* Motor de Pesquisa de Companies */}
        {searchMode === 'company' && (
          <div className="animate-in fade-in slide-in-from-bottom-4 duration-500">
            <CompanySearch onBack={() => setSearchMode(null)} />
          </div>
        )}

      </div>
    </div>
  );
}