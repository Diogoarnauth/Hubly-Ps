"use client";

import React, { createContext, useContext } from "react";
import { usePathname } from "next/navigation";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import usersService from "@/services/api/UsersService";
import CoWorkerService from "@/services/api/CoWorkerService";

type Role = 'creator' | 'company' | 'coworker' | 'justUser' | null;


interface User {
    id: number;
    name: string;
    email: string;
    role: Role;
    ownerInfo?: any;
}


interface UserContextType {
    user: User | null;
    loading: boolean;
    refreshUser: () => Promise<void>;
    logout: () => void;
}

const UserContext = createContext<UserContextType | undefined>(undefined);

export const UserProvider = ({ children }: { children: React.ReactNode }) => {

    const pathname = usePathname();

    // Não fazer fetch de user em rotas de autenticação
    const excludedUserFetchRoutes = ['/login', '/register', '/register/confirmEmail'];
    const shouldFetchUser = !excludedUserFetchRoutes.includes(pathname);

    const queryClient = useQueryClient();

    // O useQuery gere automaticamente o cache. 
    // Se o user já estiver em cache, não faz novo pedido HTTP.
    const { data: user, isLoading, refetch } = useQuery({
        queryKey: ['user'],
        queryFn: async () => {
            const userData = await usersService.getCurrentUser();
            if (!userData) return null;

            // Base do utilizador que vamos retornar
            let finalUser: User = {
                id: userData.id,
                name: userData.name,
                email: userData.email,
                role: userData.role
            };

           
            if (finalUser.role === null) {
                const coWorkerInfo = await CoWorkerService.getMyCoWorkerInfo();

                if (coWorkerInfo && coWorkerInfo.ownerId) {

                    const ownerData = await usersService.getUser(coWorkerInfo.ownerId);
                    finalUser.role = 'coworker';
                    finalUser.ownerInfo = ownerData; 

                } else {
                    finalUser.role = 'justUser';
                }
            }
            return finalUser;
        },
        enabled: shouldFetchUser,
        staleTime: 0,
        retry: false,
    });


    const logout = async () => {
        try {
            await usersService.logout();
        } catch (error) {
            console.error("Erro ao fazer logout:", error);
        } finally {
            // Limpa a cache do React Query após logout
            queryClient.setQueryData(['user'], null);
            queryClient.removeQueries({ queryKey: ['user'] });
        }
    };

    console.log("[UserProvider] Estado atual do User:", user);
    console.log("[UserProvider] Role atual:", user?.role);

    return (
        <UserContext.Provider value={{
            user: user || null,
            loading: isLoading,
            refreshUser: async () => { await refetch(); },
            logout
        }}>
            {children}
        </UserContext.Provider>
    );
};

export function useUser() {
    const context = useContext(UserContext);
    if (context === undefined) throw new Error("useUser deve ser usado dentro de um UserProvider");
    return context;
}