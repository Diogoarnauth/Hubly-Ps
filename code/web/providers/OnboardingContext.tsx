'use client';

import React, { createContext, useContext, useState } from "react";

type Role = 'creator' | 'company' | null;

interface OnboardingContextType {
    role: Role;
    setRole: (role: Role) => void;
    step: number;
    setStep: (step: number) => void;
}

const OnboardingContext = createContext<OnboardingContextType | undefined>(undefined);

export const OnboardingProvider = ({ children }: { children: React.ReactNode }) => {
    const [role, setRole] = useState<Role>(null);
    const [step, setStep] = useState(1); 

    return (
        <OnboardingContext.Provider value={{ role, setRole, step, setStep }}>
            {children}
        </OnboardingContext.Provider>
    );
};

export function useOnboardingContext() {
    const context = useContext(OnboardingContext);
    if (!context) {
        throw new Error("useOnboardingContext deve ser usado dentro de um OnboardingProvider");
    }
    return context;
}