import { SocialProfile } from '@/components/creator/SocialProfile';

export default async function CompanyProfilePage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;

  if (!id || isNaN(Number(id)) || id === 'create') {
    return null; 
  }
  
  return (
    <div className="min-h-screen bg-black p-8 md:p-10 flex justify-center">
      <div className="w-full max-w-5xl">
        
        <SocialProfile profileId={id} />
      </div>
    </div>
  );
}


