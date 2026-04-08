'use client';

import { useState } from 'react';
import { RoleSelection } from './_components/RoleSelection';
import { CreatorForm } from './_components/CreatorForm';
import { CompanyForm } from './_components/CompanyForm';

export default function OnboardingPage() {
  const [step, setStep] = useState<'SELECT' | 'CREATOR' | 'COMPANY'>('SELECT');

  return (
    <div className="min-h-screen w-full flex items-center justify-center bg-slate-50/50">
      {step === 'SELECT' && (
        <RoleSelection onSelect={(role) => setStep(role === 'creator' ? 'CREATOR' : 'COMPANY')} />
      )}
      
      {step === 'CREATOR' && (
        <CreatorForm onBack={() => setStep('SELECT')} />
      )}

      {step === 'COMPANY' && (
        <CompanyForm onBack={() => setStep('SELECT')} />
      )}
    </div>
  );
}