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
                // 1. Tenta procurar info de CoWorker
                const coWorkerInfo = await CoWorkerService.getMyCoWorkerInfo();
                console.log("coWorkerInfo recebido:", coWorkerInfo);

                // Validamos se a resposta existe e se NÃO é uma resposta de erro (ex: status 404)
                if (coWorkerInfo && coWorkerInfo.ownerId && coWorkerInfo.status !== 404) {

                    // 2. CASO SUCESSO (Status 200): Vamos buscar os dados do Owner
                    const ownerData = await usersService.getUser(coWorkerInfo.ownerId);
                    console.log("ownerDataaaaaaaaaaaaaa", ownerData);

                    finalUser.role = 'coworker';
                    finalUser.ownerInfo = ownerData; // Guarda as infos do Owner aqui

                } else {
                    console.log("CoWorker não encontrado ou erro detetado. Atribuindo 'justUser'...");

                    // Verifica se veio o status 404 ou a propriedade de erro do teu JSON
                    const isNotFoundError = !coWorkerInfo || coWorkerInfo.status === 404 || coWorkerInfo.status === "404";
                    console.log("coWorkerInfo.status:", coWorkerInfo, "isNotFoundError:", isNotFoundError);

                    if (isNotFoundError) {
                        finalUser.role = 'justUser';
                    } else {
                        // Se for outro status de erro que não o 404 (ex: 500), podes decidir o que fazer.
                        // Por segurança, atribuímos 'justUser' ou mantemos null.
                        finalUser.role = 'justUser';
                    }
                }
            }
            return finalUser;
        },
        enabled: shouldFetchUser,
        staleTime: 1000 * 60 * 30,
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