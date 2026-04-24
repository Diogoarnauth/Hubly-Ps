'use client';
import React, { useEffect, useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { ArrowLeft, Star, Plus, Settings, UserCircle, Loader2, ExternalLink } from 'lucide-react';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { toastSuccess, toastError } from '../ToastImplementations';
import usersService from '@/services/api/UsersService';
import creatorService from '@/services/api/CreatorService';
import { FullUserProfileOutputModel } from '@/services/DTO/FullUserProfileOutputModel';
import GetCreatorOutputModel from '@/services/DTO/GetCreatorOutputModel';
import { EditCreatorModal } from './EditCreatorModal';

interface CreatorProfileProps {
  id: string;
}

export function CreatorProfile({ id }: CreatorProfileProps) {
  const [profile, setProfile] = useState<FullUserProfileOutputModel | null>(null);
  const [creator, setCreator] = useState<GetCreatorOutputModel | null>(null);
  const [loading, setLoading] = useState(true);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [hoverRating, setHoverRating] = useState(0);
  const [isSubmittingRating, setIsSubmittingRating] = useState(false);
  const [userRating, setUserRating] = useState<number>(0);
  const router = useRouter();


  const handleRate = async (rating: number) => {
    if (!creator) return;

    setIsSubmittingRating(true);
    try {
      const result = await creatorService.rateCreator(parseInt(id), rating);
      console.log("result", result)
      if (result) {
        toastSuccess('Success', 'Rating submitted successfully!');
        fetchProfile();
      }
    } catch (error) {
      console.error("Error rating creator:", error);
      toastError('Error', 'Failed to submit rating');
    } finally {
      setIsSubmittingRating(false);
    }
  }

  const fetchProfile = async () => {
    try {
      const userId = parseInt(id);
      setLoading(true);

      const profileData = await usersService.getFullCreatorProfile(userId);

      if (profileData?.creator) {
        setProfile(profileData);
        setCreator(profileData.creator);

        if (!profileData.isOwner) {
          try {
            console.log("userId", userId)
            const ratingData = await creatorService.getMyRatingForCreator(userId);
            setUserRating(ratingData || 0);
          } catch (ratingError) {
            console.error("Erro ao obter rating:", ratingError);
            setUserRating(0);
          }
        } else {
          setUserRating(0);
        }

        setLoading(false);
      } else {
        toastError('Error', 'Creator not found');
        router.push('/dashboard');
      }
    } catch (error) {
      console.error("Erro ao carregar dados:", error);
      toastError('Error', 'Failed to load profile details');
      router.push('/dashboard');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    console.log("useEffect")
    console.log("id", id)
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
    <div className="text-white relative">
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

      {/* Header do Perfil */}
      <div className="flex flex-col items-center mb-12">
        <div className="w-32 h-32 bg-[#444] rounded-full flex items-center justify-center mb-3">
          <UserCircle className="w-24 h-24 text-zinc-400" />
        </div>
        <h1 className="text-2xl font-semibold">Creator Profile</h1>
        <div className="w-full max-w-5xl opacity-50 h-[1px] bg-zinc-500 mt-2"></div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        {/* Card 1: Informação do Utilizador */}
        <Card className="bg-[#414141] border-none text-white rounded-[25px]">
          <CardContent className="p-8 space-y-4">
            <h3 className="text-zinc-400 font-bold uppercase text-xs tracking-widest mb-4">User Details</h3>
            <p className="text-xl font-light flex items-center gap-3">
              <span className="font-bold">Name:</span> {profile?.name}
            </p>
            <p className="text-xl font-light flex items-center gap-3">
              <span className="font-bold">Email:</span> {profile?.email}
            </p>
          </CardContent>
        </Card>

        {/* Card 2: Detalhes do Creator e Estatísticas */}
        <Card className="bg-[#414141] border-none text-white rounded-[25px]">
          <CardContent className="p-8 space-y-4">
            <h3 className="text-zinc-400 font-bold uppercase text-xs tracking-widest mb-4">Creator Details</h3>
            <p className="text-xl font-light"><span className="font-bold">Artistic Name:</span> {creator?.artisticName || "N/A"}</p>
            <p className="text-xl font-light"><span className="font-bold">IsVerified:</span> {creator?.isVerified ? "Yes" : "No"}</p>
            <p className="text-xl font-light"><span className="font-bold">Availability Status:</span> {creator?.availabilityStatus || "N/A"}</p>
            <p className="text-xl font-light"><span className="font-bold">Global Rating:</span> {creator?.globalRating || "N/A"}</p>
            <p className="text-xl font-light"><span className="font-bold">Ratings Count:</span> {creator?.ratingsCount || "0"}</p>
            <p className="text-xl font-light"><span className="font-bold">Chats Started:</span> {creator?.chatsStartedCount || "0"}</p>
            <p className="text-xl font-light"><span className="font-bold">Chats Responded:</span> {creator?.chatsRespondedCount || "0"}</p>

            <div className="mt-6">
              <p className="text-center text-[10px] mb-2">%{responseRate.toFixed(1)} Response Rate Statistics</p>
              <div className="w-full bg-zinc-600 h-1.5 rounded-full">
                <div
                  className="bg-[#A78BFA] h-full rounded-full transition-all duration-500"
                  style={{ width: `${responseRate}%` }}
                />
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Secção de Avaliação - Apenas para visitantes */}
      {!profile?.isOwner && (
        <div className="mt-6 pt-6 border-t border-zinc-700">
          <p className="text-sm text-zinc-400 mb-3 font-medium">Rate this creator:</p>
          <div className="flex items-center gap-1">
            {[1, 2, 3, 4, 5].map((star) => {
              // LÓGICA DE PREENCHIMENTO:
              // Se houver hover, usamos o hoverRating. 
              // Se não houver hover (0), usamos o userRating que veio da API.
              const effectiveRating = hoverRating > 0 ? hoverRating : userRating;
              const isFilled = star <= effectiveRating;

              return (
                <button
                  key={star}
                  disabled={isSubmittingRating}
                  onMouseEnter={() => setHoverRating(star)}
                  onMouseLeave={() => setHoverRating(0)}
                  onClick={() => handleRate(star)}
                  className="transition-transform hover:scale-110 disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  <Star
                    size={28}
                    className={`${isFilled
                      ? "fill-[#A78BFA] text-[#A78BFA]"
                      : "text-zinc-500 fill-transparent"
                      } transition-colors duration-200`}
                  />
                </button>
              );
            })}
            {isSubmittingRating && (
              <Loader2 className="w-4 h-4 animate-spin ml-2 text-zinc-500" />
            )}
          </div>
          {userRating > 0 && (
            <p className="text-[10px] text-zinc-500 mt-2 italic">
              You previously rated this creator {userRating} stars.
            </p>
          )}
        </div>
      )}

      {/* Secção de Social Profiles */}
      <div className="mt-12 w-full max-w-5xl mx-auto">
        <h2 className="text-xl font-semibold mb-6 text-zinc-400 border-l-4 border-[#A78BFA] pl-4">
          Social Profiles
        </h2>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          {/* 1. Mapear perfis existentes */}
          {creator?.socialProfiles && creator.socialProfiles.map((social, index) => (
            <Link
              key={social.id || index}
              href={`/socialProfile/${social.id}`}
              className="group"
            >
              <Card className="bg-[#2A2A2A] border-none text-white rounded-[15px] hover:bg-[#333] transition-all duration-300 transform hover:-translate-y-1 cursor-pointer overflow-hidden h-full">
                <CardContent className="p-5 flex flex-col items-center text-center space-y-2">
                  <div className="flex items-center justify-between w-full">
                    <span className="text-[10px] uppercase tracking-widest text-purple-400 font-bold">
                      {social.platformName}
                    </span>
                    <ExternalLink className="w-3 h-3 text-zinc-500 group-hover:text-white" />
                  </div>
                  <p className="text-sm font-medium truncate w-full">
                    {social.platformUserName}
                  </p>
                  <p className="text-sm font-medium truncate w-full">
                    Followers: {social.followersCount.toLocaleString()}
                  </p>
                </CardContent>
              </Card>
            </Link>
          ))}

          {/* 2. CARD DE ADICIONAR*/}
          {profile?.isOwner && (<Card
            onClick={() => router.push(`/create-social-profile`)}
            className="bg-transparent border-2 border-dashed border-zinc-800 text-zinc-500 rounded-[15px] hover:border-[#A78BFA] hover:text-[#A78BFA] transition-all duration-300 transform hover:-translate-y-1 cursor-pointer overflow-hidden h-full min-h-[140px] flex items-center justify-center group active:scale-95"
          >
            <CardContent className="p-0 flex flex-col items-center justify-center space-y-2">
              <div className="p-3 rounded-full bg-zinc-900 group-hover:bg-[#A78BFA]/10 transition-colors">
                <Plus size={32} strokeWidth={2.5} />
              </div>
              <span className="text-xs font-bold uppercase tracking-wider">Add Profile</span>
            </CardContent>
          </Card>
          )}
        </div>

        {/* Caso não existisse nenhum perfil e quisesses manter a mensagem (opcional) */}
        {(!creator?.socialProfiles || creator.socialProfiles.length === 0) && (
          <p className="text-zinc-600 italic text-center mt-4 text-sm">No profiles linked yet.</p>
        )}
      </div>

      {/* Modal de Edição */}
      {isEditModalOpen && (
        <EditCreatorModal
          currentUsername={profile?.name || ""}
          currentArtisticName={creator?.artisticName || ""}
          currentStatus={creator?.availabilityStatus || "AVAILABLE"}
          onClose={() => setIsEditModalOpen(false)}
          onSuccess={fetchProfile}
        />
      )}
    </div>
  );
}