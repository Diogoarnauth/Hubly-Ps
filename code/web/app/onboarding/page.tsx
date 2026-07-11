'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useOnboardingContext } from '@/providers/OnboardingContext';
import { useQueryClient } from '@tanstack/react-query';
import { useUser } from '../../providers/UserProvider';
import { RoleSelection } from './_components/RoleSelection';
import { CreatorForm } from '../../components/auth/CreatorForm';
import { CompanyForm } from '../../components/auth/CompanyForm';
import authService from '@/services/api/UsersService'; 

export default function OnboardingPage() {
  const { role, setRole } = useOnboardingContext();
  const { user } = useUser();
  const queryClient = useQueryClient();
  const router = useRouter();
  const [isChecking, setIsChecking] = useState(true);

  useEffect(() => {
    async function verifyUserStatus() {
      const hasProfile = await authService.checkHasProfile();
      if (hasProfile) {
        router.push('/');
      } else {
        setIsChecking(false);
      }
    }
    verifyUserStatus();
  }, [router]);

  // Efeito para o registo automático de Coworker
  useEffect(() => {
    async function handleCoworkerSetup() {
      if (role === 'coworker' && user) {
        try {
                    
          router.push('/');
          
        } catch (error) {
          console.error("Erro no setup de coworker:", error);
          setRole(null);
        }
      }
    }
    handleCoworkerSetup();
  }, [role, user, queryClient, router, setRole]);

  if (isChecking) return <div className="flex min-h-svh items-center justify-center animate-pulse">Loading...</div>;

  return (
    <div className="flex min-h-svh w-full items-center justify-center p-6">
      <div className="w-full max-w-2xl">
        {!role && <RoleSelection onSelect={setRole} />}
        
        {role === 'creator' && <CreatorForm onBack={() => setRole(null)} />}
        {role === 'company' && <CompanyForm onBack={() => setRole(null)} />}
        
        {/* Coworker não precisa de componente visual, o useEffect trata tudo */}
      </div>
    </div>
  );
}