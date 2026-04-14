'use client';
import React, { useEffect, useState } from 'react';
import Link from 'next/link';
import { Settings, UserCircle, Loader2, ExternalLink } from 'lucide-react';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import usersService from '@/services/api/UsersService';
import { FullUserProfileOutputModel } from '@/services/DTO/FullUserProfileOutputModel';
import GetCreatorOutputModel from '@/services/DTO/GetCreatorOutputModel';

interface CreatorProfileProps {
  id: string;
}

export function CreatorProfile({ id }: CreatorProfileProps) {
  const [profile, setProfile] = useState<FullUserProfileOutputModel | null>(null);
  const [creator, setCreator] = useState<GetCreatorOutputModel | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchProfile() {
      try {
        const userId = parseInt(id);
        const data = await usersService.getFullCreatorProfile(userId);
        if (data) {
          setProfile(data);
          setCreator(data.creator); // data.creator já é uma instância de GetCreatorOutputModel (se fizeste o 'new' no service)
        }
      } catch (error) {
        console.error("Erro ao carregar perfil unificado:", error);
      } finally {
        setLoading(false);
      }
    }

    fetchProfile();
  }, [id]);

  if (loading) {
    return (
      <div className="flex min-h-[400px] items-center justify-center text-white">
        <Loader2 className="w-8 h-8 animate-spin" />
      </div>
    );
  }

  const responseRate = creator && creator.chatsStartedCount > 0
    ? (creator.chatsRespondedCount / creator.chatsStartedCount) * 100
    : 0;

  return (
    <div className="text-white">
      <div className="flex justify-end mb-4">
        <Button variant="ghost" size="icon" className="hover:bg-zinc-800">
          <Settings className="w-8 h-8 text-white" />
        </Button>
      </div>
      <div className="flex flex-col items-center mb-12">
        <div className="w-32 h-32 bg-[#444] rounded-full flex items-center justify-center mb-3">
          <UserCircle className="w-24 h-24 text-zinc-400" />
        </div>
        <h1 className="text-2xl font-semibold">Profile</h1>
        <div className="w-full max-w-5xl opacity-50 h-[1px] bg-zinc-500 mt-2"></div>
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        {/* Info do User */}
        <Card className="bg-[#414141] border-none text-white rounded-[25px]">
          <CardContent className="p-8 space-y-4">
            <p className="text-xl font-light"><span className="font-bold">Name:</span> {profile?.name}</p>
            <p className="text-xl font-light"><span className="font-bold">Email:</span> {profile?.email}</p>
          </CardContent>
        </Card>
        {/* Info do Creator */}
        <Card className="bg-[#414141] border-none text-white rounded-[25px]">
          <CardContent className="p-6 text-sm space-y-2">
            <p className="text-xl font-light"><span className="font-bold">ArtisticName:</span> {creator?.artisticName}</p>
            <p className="text-xl font-light"><span className="font-bold">IsVerified:</span> {creator?.isVerified}</p>
            <p className="text-xl font-light"><span className="font-bold">Availability_Status:</span> {creator?.availabilityStatus}</p>
            <p className="text-xl font-light"><span className="font-bold">Global Rating:</span> {creator?.globalRating}</p>
            <p className="text-xl font-light"><span className="font-bold"> chatsCount:</span> {creator?.chatsStartedCount}</p>
            <div className="mt-6">
                <p className="text-center text-[10px] mb-2">
                    %{responseRate.toFixed(1)} chatsRespondedEstatistic
                </p>
                <div className="w-full bg-zinc-400 h-1.5 rounded-full">
                    <div 
                        className="bg-[#A78BFA] h-full rounded-full transition-all duration-500" 
                        style={{ width: `${responseRate}%` }}
                    />
                </div>
            </div>
          </CardContent>
        </Card>
      </div>
    {/* Secção de Social Profiles (Quadrados como Botões) */}
      <div className="mt-12 w-full max-w-5xl mx-auto">
        <h2 className="text-xl font-semibold mb-6 text-zinc-400 border-l-4 border-[#A78BFA] pl-4">
          Social Profiles
        </h2>
        
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          {creator?.socialProfiles && creator.socialProfiles.length > 0 ? (
            creator.socialProfiles.map((social, index) => (
              <Link
                key={index} 
                href={social.url?.startsWith('http') ? social.url : `https://${social.url || ''}`}
                target="_blank"
                className="group"
              >
                <Card className="bg-[#2A2A2A] border-none text-white rounded-[15px] hover:bg-[#333] transition-all duration-300 transform hover:-translate-y-1 cursor-pointer overflow-hidden">
                  <CardContent className="p-5 flex flex-col items-center text-center space-y-2">
                    <div className="flex items-center justify-between w-full">
                      <span className="text-[10px] uppercase tracking-widest text-purple-400 font-bold">
                        {social.platformName}
                      </span>
                      <ExternalLink className="w-3 h-3 text-zinc-500 group-hover:text-white" />
                    </div>
                    <p className="text-sm font-medium truncate w-full">
                      {creator.artisticName}
                    </p>
                    <p className="text-[10px] text-zinc-500 truncate w-full italic">
                      {(social.url || '').replace(/^https?:\/\//, '')}
                    </p>
                  </CardContent>
                </Card>
              </Link>
            ))
          ) : (
            <div className="col-span-full py-10 border-2 border-dashed border-zinc-800 rounded-[20px] text-center">
              <p className="text-zinc-600 italic">No social profiles found for this creator.</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}