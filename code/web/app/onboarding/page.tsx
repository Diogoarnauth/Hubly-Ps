'use client';

import { useOnboardingContext } from '@/providers/OnboardingContext';
import { RoleSelection } from './_components/RoleSelection';
import { CreatorForm } from './_components/CreatorForm';
import { CompanyForm } from './_components/CompanyForm';

export default function OnboardingPage() {
  const { role, setRole } = useOnboardingContext();

  return (
    <div className="flex min-h-svh w-full items-center justify-center p-6 md:p-10">
      <div className="w-full max-w-2xl"> {}
        
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