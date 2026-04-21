'use client';

import React, { createContext, useContext, useEffect, useState } from "react";
import usersService from "@/services/api/UsersService";

interface User {
    id: number;
    name: string;
    email: string;
    role: 'creator' | 'company' | null;
}

interface UserContextType {
    user: User | null;
    loading: boolean;
    refreshUser: () => Promise<void>; 
    logout: () => void;
}

const UserContext = createContext<UserContextType | undefined>(undefined);

export const UserProvider = ({ children }: { children: React.ReactNode }) => {
    const [user, setUser] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);

    const fetchUser = async () => {
        try {
            const userData = await usersService.getCurrentUser();
            console.log("userData", userData)
            if (userData) {
                const userToSave: User = {
                    id: userData.id,
                    name: userData.name,
                    email: userData.email,
                    role: userData.role
                };
                setUser(userToSave);
                localStorage.setItem('hubly_user', JSON.stringify(userToSave));
            } else {
                setUser(null);
                localStorage.removeItem('hubly_user');
            }
        } catch (error) {
            console.error("Erro ao carregar utilizador:", error);
            setUser(null);
            localStorage.removeItem('hubly_user');
        }
    };

    useEffect(() => {
        const savedUser = localStorage.getItem('hubly_user');
        if (savedUser) {
            setUser(JSON.parse(savedUser));
            setLoading(false);
        } else {
            fetchUser().finally(() => setLoading(false));
        }
    }, []);

   const logout = async () => {
        try {
            // 1. Chama a API para fazer logout no servidor
            await usersService.logout(); 
        } catch (error) {
            console.error("Erro ao fazer logout na API:", error);
        } finally {
            setUser(null);
            localStorage.removeItem('hubly_user');
        }
    };

    return (
        <UserContext.Provider value={{ 
            user, 
            loading, 
            refreshUser: fetchUser, 
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