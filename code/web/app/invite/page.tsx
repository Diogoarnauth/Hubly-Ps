import { InvitePage } from '@/components/invites/InvitesPage';

export default function InvitePageWrapper() {
  return (
    <div className="min-h-screen bg-black p-8 md:p-10 flex justify-center">
      <div className="w-full max-w-5xl">
        <InvitePage />
      </div>
    </div>
  );
}
