'use client';
import React, { useEffect, useState } from 'react';
import { ArrowLeft, Building2, Loader2, Settings } from 'lucide-react';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import usersService from '@/services/api/UsersService';
import { useRouter } from 'next/navigation';
import { toastError } from '../ToastImplementations';
import { FullCompanyProfileOutputModel } from '@/services/DTO/FullCompanyProfileOutputModel';
import { EditCompanyModal } from './EditCompanyModal';
import GetCompanyOutputModel from '@/services/DTO/GetCompanyOutputModel';

interface CompanyProfileProps {
  id: string;
}

export function CompanyProfile({ id }: CompanyProfileProps) {
  const [profile, setProfile] = useState<FullCompanyProfileOutputModel | null>(null);
  const [company, setCompany] = useState<GetCompanyOutputModel | null>(null);
  const [loading, setLoading] = useState(true);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const router = useRouter();


  const fetchProfile = async () => {
    try {
      const userId = parseInt(id);
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
  };

  useEffect(() => {
    console.log("useEffect")
    console.log("id", id)
    fetchProfile();
  }, []);

  if (loading) {
    return (
      <div className="flex min-h-[400px] items-center justify-center text-white">
        <Loader2 className="w-8 h-8 animate-spin" />
      </div>
    );
  }

  return (
    <div className="text-white relative pt-[5vh]">
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
          initialData={profile?.company}
          onClose={() => setIsEditModalOpen(false)}
          onSuccess={fetchProfile}
        />
      )}
    </div>
  );
}