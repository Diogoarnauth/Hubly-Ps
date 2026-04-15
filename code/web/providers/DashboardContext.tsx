'use client';

import React, { createContext, useContext, useState } from "react";

// Removi a sidebar e deixei o espaço para futuros estados globais do dashboard
interface DashboardContextType {
    isLoading: boolean;
    setIsLoading: (loading: boolean) => void;
}

const DashboardContext = createContext<DashboardContextType | undefined>(undefined);

export const DashboardProvider = ({ children }: { children: React.ReactNode }) => {
    const [isLoading, setIsLoading] = useState(false);

    return (
        <DashboardContext.Provider value={{ isLoading, setIsLoading }}>
            {children}
        </DashboardContext.Provider>
    );
};

export function useDashboardContext() {
    const context = useContext(DashboardContext);
    if (!context) {
        throw new Error("useDashboardContext deve ser usado dentro de um DashboardProvider");
    }
    return context;
}