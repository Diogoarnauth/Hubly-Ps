import { JustUserProfile } from '@/components/justUser/JustUserProfile';

export default function JustUserProfilePage() {
  return (
    <div className="min-h-screen bg-black p-8 md:p-10 flex justify-center">
      <div className="w-full max-w-5xl">
        <JustUserProfile />
      </div>
    </div>
  );
}