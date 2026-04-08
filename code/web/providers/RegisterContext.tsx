'use client';

import React, { createContext, useContext, useState, useEffect } from "react";

const RegisterContext = createContext({
    email: undefined as string | undefined,
    setEmail: (_: string) => {},
});

export const RegisterProvider = ({ children }: { children: React.ReactNode }) => {
    // Inicializa o estado tentando ler do sessionStorage
    const [email, setEmailState] = useState<string | undefined>(() => {
        if (typeof window !== 'undefined') {
            return sessionStorage.getItem('hubly_register_email') || undefined;
        }
        return undefined;
    });

    const setEmail = (newEmail: string) => {
        setEmailState(newEmail);
        sessionStorage.setItem('hubly_register_email', newEmail);
    };

    return (
        <RegisterContext.Provider value={{ email, setEmail }}>
            {children}
        </RegisterContext.Provider>
    );
};

export function useRegisterContext(): [string | undefined, (email: string) => void] {
    const { email, setEmail } = useContext(RegisterContext);
    return [email, setEmail];
}