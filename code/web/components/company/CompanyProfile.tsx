'use client';
import React, { useCallback, useEffect, useState } from 'react';
import { ArrowLeft, Building2, Loader2, Settings, Send } from 'lucide-react';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { toastError, toastSuccess } from '../ToastImplementations';
import usersService, { UserInfo } from '@/services/api/UsersService';
import conversationService from '@/services/api/ConversationService';
import { useRouter } from 'next/navigation';
import { FullCompanyProfileOutputModel } from '@/services/DTO/FullCompanyProfileOutputModel';
import { SocialProfileOutputModel } from '@/services/DTO/GetCreatorOutputModel';
import { EditCompanyModal } from './EditCompanyModal';
import GetCompanyOutputModel from '@/services/DTO/company/GetCompanyOutputModel';
import CompanyProfileProps from '@/services/DTO/creator/CreatorChatSelectionPros';

export function CompanyProfile({ id }: CompanyProfileProps) {
  const [profile, setProfile] = useState<FullCompanyProfileOutputModel | null>(null);
  const [company, setCompany] = useState<GetCompanyOutputModel | null>(null);
  const [loading, setLoading] = useState(true);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [currentUser, setCurrentUser] = useState<UserInfo | null>(null);
  const [currentCompanyId, setCurrentCompanyId] = useState<number | null>(null);
  const [checkingChat, setCheckingChat] = useState(false);
  const [dropdownLoadingProfileId, setDropdownLoadingProfileId] = useState<number | null>(null);
  const [mySocialProfiles, setMySocialProfiles] = useState<{ id: number; name: string; platform: string }[]>([]);
  const [showDropdown, setShowDropdown] = useState(false);
  const router = useRouter();


  const fetchProfile = useCallback(async () => {
    try {
      const userId = parseInt(id || '0');
      const data = await usersService.getFullCompanyProfile(userId);

      if (data?.company) {
        setProfile(data);
        setCompany(data.company);
        setLoading(false);
      } else {
        toastError('Company not found', 'Invalid id');
        await new Promise(resolve => setTimeout(resolve, 1500));
        router.push('/dashboard');
      }
    } catch (error) {
      console.error("Erro ao carregar perfil:", error);
      toastError('Error', 'Failed to load profile');
      router.push('/dashboard');
    }
  }, [id, router]);

  useEffect(() => {
    fetchProfile();
  }, [fetchProfile]);

  useEffect(() => {
    const loadCurrentUser = async () => {
      try {
        const myInfo = await usersService.getCurrentUser();
        if (!myInfo) return;

        setCurrentUser(myInfo);

        if (myInfo.role === 'creator') {
          const myProfile = await usersService.getFullCreatorProfile(myInfo.id);
          if (myProfile?.creator?.socialProfiles) {
            const profiles = myProfile.creator.socialProfiles.map((sp: SocialProfileOutputModel) => ({
              id: sp.id,
              name: sp.platformUserName,
              platform: sp.platformName,
            }));
            setMySocialProfiles(profiles);
          }
        }

        if (myInfo.role === 'company') {
          const myCompanyProfile = await usersService.getFullCompanyProfile(myInfo.id);
          if (myCompanyProfile?.company?.id) {
            setCurrentCompanyId(myCompanyProfile.company.id);
          }
        }
      } catch (error) {
        console.error('Error loading current user or profiles:', error);
      }
    };

    if (profile && profile.isOwner === false) {
      loadCurrentUser();
    }
  }, [profile]);

  const handleConversationFlow = async (senderProfileId: number, senderType: number) => {
    try {
      setCheckingChat(true);
      setDropdownLoadingProfileId(senderProfileId);

      const receiverCompanyId = company?.id ?? parseInt(id);

      const checkResult = await conversationService.checkConversationExists(
        senderProfileId,
        senderType,
        receiverCompanyId,
        1
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
          ProfileId: receiverCompanyId,
          Type: 1,
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
    <div className="text-white relative pt-[5vh]">
          {!profile?.isOwner && (
            <div className="flex justify-end mb-4 gap-2">
              <div className="relative">
                <Button
                  variant="ghost"
                  size="icon"
                  className="hover:bg-zinc-800"
                  disabled={
                    checkingChat ||
                    !currentUser ||
                    (currentUser.role === 'creator' && !mySocialProfiles.length) ||
                    (currentUser.role === 'company' && !currentCompanyId)
                  }
                  onClick={async () => {
                    if (currentUser?.role === 'creator') {
                      setShowDropdown((prev) => !prev);
                      return;
                    }

                    if (currentUser?.role === 'company' && currentCompanyId) {
                      setShowDropdown(false);
                      await handleConversationFlow(currentCompanyId, 1);
                    }
                  }}
                  title={
                    checkingChat
                      ? 'Loading...'
                      : !currentUser
                      ? 'Loading...'
                      : currentUser.role === 'creator'
                      ? mySocialProfiles.length
                        ? 'Start conversation'
                        : 'No profiles loaded'
                      : currentCompanyId
                      ? 'Start conversation'
                      : 'Loading...'
                  }
                >
                  <Send className="w-8 h-8 text-white" />
                </Button>
                {showDropdown && currentUser?.role === 'creator' && mySocialProfiles.length > 0 && (
                  <div className="absolute top-full right-0 mt-2 w-64 bg-[#414141] border border-zinc-600 rounded-lg shadow-lg z-10">
                    <div className="p-3">
                      <p className="text-sm text-zinc-300 mb-2">Queres iniciar este chat com qual destes teus social profiles?</p>
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
            </div>
          )}
          {profile?.isOwner && (
            <div className="flex justify-end mb-4">
              <Button 
                variant="ghost" 
                size="icon" 
                className="hover:bg-zinc-800"
                onClick={() => setIsEditModalOpen(true)}
              >
                <Settings className="w-8 h-8 text-white" />
              </Button>
            </div>
          )}

      {/* Botão de Voltar */}
      <div className="flex justify-start mb-4">
        <Button
          variant="ghost"
          className="text-zinc-400 hover:text-white hover:bg-zinc-800 gap-2"
          onClick={() => router.back()}
        >
          <ArrowLeft className="w-4 h-4" /> Back to Profile
        </Button>
      </div>

      <div className="flex flex-col items-center mb-12">
        <div className="w-32 h-32 bg-zinc-800 rounded-full flex items-center justify-center mb-3">
          <Building2 className="w-16 h-16 text-zinc-400" />
        </div>
        <h1 className="text-2xl font-semibold">Company Profile</h1>
        <div className="w-full max-w-5xl opacity-50 h-[1px] bg-zinc-500 mt-2"></div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        <Card className="bg-[#414141] border-none text-white rounded-[25px]">
          <CardContent className="p-8 space-y-4">
            <h3 className="text-zinc-400 font-bold uppercase text-[10px] tracking-widest mb-4">User Details</h3>
            <p className="text-xl font-light"><span className="font-bold">Name:</span> {profile?.name}</p>
            <p className="text-xl font-light"><span className="font-bold">Email:</span> {profile?.email}</p>
          </CardContent>
        </Card>

        <Card className="bg-[#414141] border-none text-white rounded-[25px]">
          <CardContent className="p-8 space-y-4">
            <h3 className="text-zinc-400 font-bold uppercase text-[10px] tracking-widest mb-4">Business Details</h3>
            <p className="text-xl font-light"><span className="font-bold">Company Name:</span> {profile?.company?.companyName || "N/A"}</p>
            <p className="text-xl font-light"><span className="font-bold">Verified:</span> {profile?.company?.isVerified ? "Yes" : "No"}</p>
            <p className="text-xl font-light"><span className="font-bold">Description:</span> {profile?.company?.description || "N/A"}</p>
            <p className="text-xl font-light">
              <span className="font-bold">Sectors:</span> {profile?.company?.sectors?.join(", ") || "N/A"}
            </p>
            <p className="text-xl font-light"><span className="font-bold">Company Size:</span> {profile?.company?.companySize || "N/A"}</p>
            <p className="text-xl font-light"><span className="font-bold">Website:</span> {profile?.company?.websiteLink || "N/A"}</p>
          </CardContent>
        </Card>
      </div>

      {isEditModalOpen && (
        <EditCompanyModal
          currentUsername={profile?.name || ''}
          initialData={profile?.company}
          onClose={() => setIsEditModalOpen(false)}
          onSuccess={fetchProfile}
        />
      )}
    </div>
  );
}