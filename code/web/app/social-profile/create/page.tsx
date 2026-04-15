'use client';

import { SocialProfileForm } from '@/components/auth/SocialProfileForm';
import { Button } from '@/components/ui/button';
import { useRouter } from 'next/navigation';
import { ChevronLeft } from 'lucide-react';

export default function CreateSocialProfilePage() {
  const router = useRouter();

  return (
    <div className="space-y-6">
      {/* Botão para voltar ao Dashboard ou Perfil */}
      <Button 
        variant="ghost" 
        size="sm" 
        onClick={() => router.back()} 
        className="text-muted-foreground hover:text-white mb-4"
      >
        <ChevronLeft className="mr-2 h-4 w-4" />
        Back
      </Button>

      <div className="space-y-2">
        <h1 className="text-3xl font-bold tracking-tight italic">
          Add <span className="text-primary">Social Profile</span>
        </h1>
        <p className="text-muted-foreground">
          Connect a new social network to your Hubly account.
        </p>
      </div>
      
      <SocialProfileForm />
    </div>
  );
}