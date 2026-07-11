import { CreatorProfile } from '@/components/creator/CreatorProfile';

export default async function CreatorProfilePage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  
  return (
    <div className="min-h-screen bg-black p-8 md:p-10 flex justify-center">
      <div className="w-full max-w-5xl">
        <CreatorProfile id={id} />
      </div>
    </div>
  );
}

