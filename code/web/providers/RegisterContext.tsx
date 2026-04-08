'use client';

import React, { createContext, useContext, useState } from "react";

const RegisterContext = createContext({
    email: undefined as string | undefined,
    setEmail: (_: string) => {},
});

export const RegisterProvider = ({ children }: { children: React.ReactNode }) => {
    const [email, setEmail] = useState<string | undefined>(undefined);

    const value = {
        email,
        setEmail,
    };

    return (
        <RegisterContext.Provider value={value}>
            {children}
        </RegisterContext.Provider>
    );
};

export function useRegisterContext(): [string | undefined, (email: string) => void] {
    const { email, setEmail } = useContext(RegisterContext);
    return [email, setEmail];
}
