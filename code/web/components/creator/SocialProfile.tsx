'use client';
import React, { useEffect, useState, useCallback, useRef } from 'react';
import { useRouter } from 'next/navigation';
import { Trash2, Settings, Users, Tag, DollarSign, FileText, Loader2, ArrowLeft, ExternalLink, Send } from 'lucide-react';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { toastSuccess, toastError } from '../ToastImplementations';
import creatorService from '@/services/api/CreatorService';
import usersService, { UserInfo } from '@/services/api/UsersService';
import conversationService from '@/services/api/ConversationService';
import { GetSocialProfileOutputModel } from '@/services/DTO/GetSocialProfileOutputModel';
import { SocialProfileOutputModel } from '@/services/DTO/GetCreatorOutputModel';
import { EditSocialProfileModal } from './EditSocialProfileModal';
import SocialProfileProps from '@/services/DTO/creator/SocialProfilePropsInputModel';


export function SocialProfile({ profileId }: SocialProfileProps) {
  const [data, setData] = useState<GetSocialProfileOutputModel | null>(null);
  const [loading, setLoading] = useState(true);
  const [isEditSocialProfileModal, setIsEditSocialProfileModal] = useState(false);
  const [checkingChat, setCheckingChat] = useState(false);
  const [dropdownLoadingProfileId, setDropdownLoadingProfileId] = useState<number | null>(null);
  const [currentUser, setCurrentUser] = useState<UserInfo | null>(null);
  const [mySocialProfiles, setMySocialProfiles] = useState<{id: number, name: string, platform: string}[]>([]);
  const [showDropdown, setShowDropdown] = useState(false);
  const router = useRouter();
  const isFetching = useRef(false);

  const handleDelete = async () => {
    const confirmDelete = window.confirm(
      "Tem mesmo a certeza que quer eliminar este perfil social? Esta operação é permanente e não pode ser desfeita."
    );

    if (!confirmDelete) return;

    try {
      const id = parseInt(profileId);
      await creatorService.deleteSocialProfile(id); 
      
      toastSuccess('Sucesso', 'Perfil social eliminado com sucesso');
      router.push('/'); 
    } catch (error) {
      console.error("Error deleting social profile:", error);
      toastError('Erro', 'Não foi possível eliminar o perfil social');
    }
  };

  const fetchSocialProfile = useCallback(async () => {
    if (isFetching.current) return;

    try {
      isFetching.current = true;
      const id = parseInt(profileId);
      const response = await creatorService.getSocialProfileById(id);

      if (response) {
        setData(response);
        setLoading(false);
      } else {
        toastError('CreateSocialProfile not found', 'Invalid id');
        await new Promise(resolve => setTimeout(resolve, 1500));
        router.push('/');
      }
    } catch (error) {
      console.error("Error loading social profile:", error);
      toastError('Error', 'Failed to load social profile');
      router.push('/');
    }
  }, [profileId, router]);

  const handleEditSuccess = async () => {
    isFetching.current = false; // Garante que o trinco está aberto
    await fetchSocialProfile(); // Procura os dados novos no servidor
    setIsEditSocialProfileModal(false); // Fecha o modal
  };

  useEffect(() => {
    isFetching.current = false;
    fetchSocialProfile();

    return () => {
      isFetching.current = true;
    };
  }, [fetchSocialProfile]);

  useEffect(() => {
    const loadCurrentUser = async () => {
      try {
        const myInfo = await usersService.getCurrentUser();
        if (!myInfo) return;

        setCurrentUser(myInfo);
        if (myInfo.role !== 'creator') {
          setMySocialProfiles([]);
          return;
        }

        const myProfile = await usersService.getFullCreatorProfile(myInfo.id);
        if (myProfile?.creator?.socialProfiles) {
          const profiles = myProfile.creator.socialProfiles.map((sp: SocialProfileOutputModel) => ({
            id: sp.id,
            name: sp.platformUserName,
            platform: sp.platformName
          }));
          setMySocialProfiles(profiles);
        }
      } catch (error) {
        console.error("Error loading current user or social profiles:", error);
      }
    };

    if (data?.isOwner === false) {
      loadCurrentUser();
    }
  }, [data?.isOwner]);

  const handleConversationFlow = async (senderProfileId: number, senderType: number) => {
    try {
      setCheckingChat(true);

      const checkResult = await conversationService.checkConversationExists(
        senderProfileId,
        senderType,
        parseInt(profileId),
        2
      );

      if (checkResult?.exists) {
        toastSuccess('Success', 'Conversation exists!');
        setShowDropdown(false);
        if (currentUser?.role === 'company') {
          router.push(`/chatsCompany/${senderProfileId}`);
        } else {
          router.push(`/chat/${senderProfileId}`);
        }
        return;
      }

      const result = await conversationService.createConversation({
        Sender: {
          ProfileId: senderProfileId,
          Type: senderType,
        },
        Receiver: {
          ProfileId: parseInt(profileId),
          Type: 2,
        },
      });

      if (result.success && result.data && 'id' in result.data) {
        toastSuccess('Success', 'Conversation started!');
        setShowDropdown(false);
        if (currentUser?.role === 'company') {
          router.push(`/chatsCompany/${senderProfileId}`);
        } else {
          router.push(`/chat/${senderProfileId}`);
        }
      } else {
        toastError('Error', result.message || 'Failed to start conversation');
      }
    } catch (error) {
      console.error('Error handling conversation flow:', error);
      toastError('Error', 'Failed to start conversation');
    } finally {
      setCheckingChat(false);
      setDropdownLoadingProfileId(null);
    }
  };

  if (loading) {
    return (
      <div className="flex min-h-[400px] items-center justify-center text-white">
        <Loader2 className="w-8 h-8 animate-spin" />
      </div>
    );
  }

  return (
    <div className="text-white relative space-y-8">
      <div className="flex justify-end mb-4 gap-2">
        {!data?.isOwner && (
          <div className="relative">
            <Button
              variant="ghost"
              size="icon"
              className="hover:bg-zinc-800"
              disabled={
                checkingChat ||
                !currentUser ||
                (currentUser.role === 'creator' && !mySocialProfiles.length)
              }
              onClick={async () => {
                if (currentUser?.role === 'creator') {
                  setShowDropdown((prev) => !prev);
                  return;
                }

                if (currentUser?.role === 'company' && currentUser.id) {
                  setShowDropdown(false);
                  await handleConversationFlow(currentUser.id, 1);
                }
              }}
              title={
                checkingChat
                  ? "Loading..."
                  : !currentUser
                  ? "Loading..."
                  : currentUser.role === 'company'
                  ? "Start conversation"
                  : currentUser.role === 'creator'
                  ? mySocialProfiles.length
                    ? "Start conversation"
                    : "No profiles loaded"
                  : "Start conversation"
              }
            >
              <Send className="w-8 h-8 text-white" />
            </Button>
            {showDropdown && currentUser?.role === 'creator' && mySocialProfiles.length > 0 && (
              <div className="absolute top-full right-0 mt-2 w-64 bg-[#414141] border border-zinc-600 rounded-lg shadow-lg z-10">
                <div className="p-3">
                  <p className="text-sm text-zinc-300 mb-2">Do you want to start a conversation with any of these social profiles?</p>
                  <div className="space-y-1">
                    {mySocialProfiles.map((profile) => (
                      <button
                        key={profile.id}
                        disabled={dropdownLoadingProfileId === profile.id}
                        className="w-full text-left p-2 rounded hover:bg-zinc-700 text-white text-sm disabled:cursor-not-allowed disabled:opacity-60"
                        onClick={async () => {
                          setDropdownLoadingProfileId(profile.id);
                          await handleConversationFlow(profile.id, 2);
                        }}
                      >
                        <div className="font-medium">{profile.name}</div>
                        <div className="text-xs text-zinc-400">{profile.platform}</div>
                      </button>
                    ))}
                  </div>
                </div>
              </div>
            )}
          </div>
        )}
        {data?.isOwner && (
          <>
            {/* Botão Delete */}
            <Button
              variant="ghost"
              size="icon"
              className="hover:bg-red-950/30 hover:text-red-500 text-zinc-400 transition-colors"
              onClick={handleDelete}
              title="Delete Social Profile"
            >
              <Trash2 className="w-6 h-6" />
            </Button>

            <Button
              variant="ghost"
              size="icon"
              className="hover:bg-zinc-800"
              onClick={() => setIsEditSocialProfileModal(true)}
            >
              <Settings className="w-8 h-8 text-white" />
            </Button>
          </>
        )}
      </div>
      {/* Botão de Voltar */}
      <div className="flex justify-start mb-4">
        <Button
          variant="ghost"
          className="text-zinc-400 hover:text-white hover:bg-zinc-800 gap-2"
          onClick={() => router.push(`../creator/${data?.creatorId}`)}
        >
          <ArrowLeft className="w-4 h-4" /> See Main Creator Profile
        </Button>
      </div>

      {/* Header do Social Profile */}
      <div className="flex flex-col items-center mb-12">
        <h1 className="text-3xl font-bold tracking-tight">@{data?.platformUserName}</h1>
        <p className="text-zinc-500 mt-2 uppercase text-xs tracking-[0.2em] font-bold">Social Platform Profile</p>
        <p className="text-zinc-500 mt-2 uppercase text-xs tracking-[0.2em] font-bold">{data?.platformName} </p>
        <div className="w-full max-w-2xl opacity-20 h-[1px] bg-gradient-to-r from-transparent via-zinc-500 to-transparent mt-6"></div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">

        {/* Card 1: Reach & Stats */}
        <Card className="bg-[#414141] border-none text-white rounded-[25px] md:col-span-1">
          <CardContent className="p-8 space-y-6">
            <div className="flex items-center gap-3 text-[#A78BFA]">
              <Users className="w-5 h-5" />
              <h3 className="font-bold uppercase text-xs tracking-widest">Audience</h3>
            </div>
            <div>
              <p className="text-4xl font-bold">{data?.followersCount.toLocaleString()}</p>
              <p className="text-zinc-400 text-sm mt-1">Total Followers</p>
            </div>

            <div className="pt-4 border-t border-zinc-700">
              <a
                href={data?.link.startsWith('http') ? data.link : `https://${data?.link}`}
                target="_blank"
                rel="noopener noreferrer"
                className="flex items-center justify-between group hover:text-[#A78BFA] transition-colors"
              >
                <span className="text-sm">Visit Profile</span>
                <ExternalLink className="w-4 h-4" />
              </a>
            </div>
          </CardContent>
        </Card>

        {/* Card 2: Pricing & Sectors */}
        <Card className="bg-[#414141] border-none text-white rounded-[25px] md:col-span-2">
          <CardContent className="p-8 space-y-8">
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-8">

              {/* Pricing Section */}
              <div className="space-y-4">
                <div className="flex items-center gap-3 text-[#A78BFA]">
                  <DollarSign className="w-5 h-5" />
                  <h3 className="font-bold uppercase text-xs tracking-widest">Pricing Range</h3>
                </div>
                <div className="flex items-baseline gap-2">
                  <span className="text-2xl font-semibold">
                    {data?.priceMin ? `${data.priceMin}€` : 'N/A'}
                  </span>
                  <span className="text-zinc-500">—</span>
                  <span className="text-2xl font-semibold">
                    {data?.priceMax ? `${data.priceMax}€` : 'N/A'}
                  </span>
                </div>
              </div>

              {/* Sectors Section */}
              <div className="space-y-4">
                <div className="flex items-center gap-3 text-[#A78BFA]">
                  <Tag className="w-5 h-5" />
                  <h3 className="font-bold uppercase text-xs tracking-widest">Sectors</h3>
                </div>
                <div className="flex flex-wrap gap-2">
                  {data?.sectors && data.sectors.length > 0 ? (
                    data.sectors.map((sector, index) => (
                      <span key={index} className="px-3 py-1 bg-zinc-800 rounded-full text-xs font-medium border border-zinc-700">
                        {sector}
                      </span>
                    ))
                  ) : (
                    <span className="text-zinc-500 italic text-sm">No sectors listed</span>
                  )}
                </div>
              </div>

            </div>

            {/* Description Section */}
            <div className="pt-8 border-t border-zinc-700 space-y-4">
              <div className="flex items-center gap-3 text-[#A78BFA]">
                <FileText className="w-5 h-5" />
                <h3 className="font-bold uppercase text-xs tracking-widest">Description / Bio</h3>
              </div>
              <p className="text-zinc-300 leading-relaxed font-light">
                {data?.description || "This creator hasn't provided a specific description for this social profile."}
              </p>
            </div>

          </CardContent>
        </Card>
      </div>
      {isEditSocialProfileModal && data && (
        <EditSocialProfileModal
          initialData={data}
          onClose={() => setIsEditSocialProfileModal(false)}
          onSuccess={handleEditSuccess}
        />
      )}
    </div>
  );
}