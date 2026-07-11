import { ChatDashboard } from "@/components/chat/ChatDashboard";

export default async function CompanyChatPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;

    return (
        <main className="min-h-screen bg-black">
            <div className="container mx-auto">
                <ChatDashboard profileId={id} isCompany={true} />
            </div>
        </main>
    );
}
