'use client';
import React, { useEffect, useState, useCallback, useRef } from 'react';
import { useRouter } from 'next/navigation';
import { Trash2, Settings, Users, Tag, DollarSign, FileText, Loader2, ArrowLeft, ExternalLink } from 'lucide-react';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { toastSuccess, toastError } from '../ToastImplementations';
import creatorService from '@/services/api/CreatorService'; 
import { GetSocialProfileOutputModel } from '@/services/DTO/GetSocialProfileOutputModel';
import { EditSocialProfileModal } from './EditSocialProfileModal';


interface SocialProfileProps {
  profileId: string;
}

export function SocialProfile({ profileId }: SocialProfileProps) {
  const [data, setData] = useState<GetSocialProfileOutputModel | null>(null);
  const [loading, setLoading] = useState(true);
  const [isEditSocialProfileModal, setIsEditSocialProfileModal] = useState(false);
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
      router.push('/dashboard'); 
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
        router.push('/dashboard');
      }
    } catch (error) {
      console.error("Error loading social profile:", error);
      toastError('Error', 'Failed to load social profile');
      router.push('/dashboard');
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

  if (loading) {
    return (
      <div className="flex min-h-[400px] items-center justify-center text-white">
        <Loader2 className="w-8 h-8 animate-spin" />
      </div>
    );
  }

  return (
    <div className="text-white relative space-y-8">
      {data?.isOwner && (
        <div className="flex justify-end mb-4">

          {/* Botão Delete */}
          <Button
            variant="ghost"
            size="icon"
            className="hover:bg-red-950/30 hover:text-red-500 text-zinc-400 transition-colors"
            onClick={handleDelete}
            title="Eliminar Perfil Social"
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
        </div>
        

        
      )}
      {/* Botão de Voltar */}
      <div className="flex justify-start mb-4">
        <Button
          variant="ghost"
          className="text-zinc-400 hover:text-white hover:bg-zinc-800 gap-2"
          onClick={() => router.push(`../creator/${data?.creatorId}`)}
        >
          <ArrowLeft className="w-4 h-4" /> Back to Profile
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