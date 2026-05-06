'use client';
import React, { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import userService from '@/services/api/UsersService';
import SocialProfile from '@/services/DTO/creator/SocialProfileOutputModel';
import  CreatorChatSelectionProps  from '@/services/DTO/creator/CreatorChatSelectionPros';

export const CreatorChatSelection = ({ id }: CreatorChatSelectionProps) => {
    const [profiles, setProfiles] = useState<SocialProfile[]>([]);
    const [loading, setLoading] = useState(true);
    const router = useRouter();

    useEffect(() => {
        const loadProfiles = async () => {
            try {
                const user = await userService.getCurrentUser();
                const fullProfile = await userService.getFullCreatorProfile(user.id);
                
                console.log("Resposta da API FullProfile:", fullProfile);
                
                if (fullProfile && fullProfile.creator && fullProfile.creator.socialProfiles) {
                    setProfiles(fullProfile.creator.socialProfiles);
                } else {
                    setProfiles([]);
                }

            } catch (err) {
                console.error("Erro ao carregar perfis sociais:", err);
            } finally {
                setLoading(false);
            }
        };
        loadProfiles();
    }, []);

    if (loading) {
        return (
            <div className="flex justify-center items-center h-64">
                <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-blue-500"></div>
            </div>
        );
    }

    return (
        <div className="space-y-6">
            <div className="text-center md:text-left">
                <h1 className="text-3xl font-bold text-white mb-2">Your Conversations</h1>
                <p className="text-zinc-400">Select a social network to manage your messages.</p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {profiles.map((profile) => (
                    <button
                        key={profile.id}
                        onClick={() => router.push(`/chat/${profile.id}`)}
                        className="flex items-center justify-between p-6 bg-zinc-900/50 border border-zinc-800 rounded-2xl hover:border-blue-500 hover:bg-zinc-800 transition-all group text-left"
                    >
                        <div className="flex flex-col">
                            <span className="text-lg font-semibold text-white group-hover:text-blue-400 transition-colors">
                                {profile.platformUserName}
                            </span>
                            <span className="text-xs text-zinc-500 uppercase tracking-widest mt-1">
                                {profile.platformName || "Platform"}
                            </span>
                        </div>
                        <div className="h-10 w-10 rounded-full bg-zinc-800 border border-zinc-700 flex items-center justify-center group-hover:bg-blue-600 group-hover:border-blue-500 transition-all">
                            <span className="text-white text-xl">→</span>
                        </div>
                    </button>
                ))}
            </div>

            {profiles.length === 0 && (
                <div className="text-center p-16 border-2 border-dashed border-zinc-800 rounded-3xl">
                    <p className="text-zinc-500">We couldn't find any social profiles associated with this account.</p>
                </div>
            )}
        </div>
    );
};