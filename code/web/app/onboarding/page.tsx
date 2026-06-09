'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useOnboardingContext } from '@/providers/OnboardingContext';
import { RoleSelection } from './_components/RoleSelection';
import { CreatorForm } from '../../components/auth/CreatorForm';
import { CompanyForm } from '../../components/auth/CompanyForm';
import authService from '@/services/api/UsersService'; 

export default function OnboardingPage() {
  const { role, setRole } = useOnboardingContext();
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

  if (isChecking) {
    return (
      <div className="flex min-h-svh w-full items-center justify-center">
        <div className="animate-pulse text-muted-foreground text-lg">
          Loading your profile...
        </div>
      </div>
    );
  }

  return (
    <div className="flex min-h-svh w-full items-center justify-center p-6 md:p-10">
      <div className="w-full max-w-2xl">
        
        {!role && (
          <RoleSelection onSelect={(selectedRole) => setRole(selectedRole)} />
        )}
        
        {role === 'creator' && (
          <CreatorForm onBack={() => setRole(null)} />
        )}

        {role === 'company' && (
          <CompanyForm onBack={() => setRole(null)} />
        )}

      </div>
    </div>
  );
}