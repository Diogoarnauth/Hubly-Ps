import { ChatDashboard } from "@/components/chat/ChatDashboard";

export default function CompanyChatPage({ params }: { params: { id: string } }) {

    return (
        <main className="min-h-screen bg-black">
            <div className="container mx-auto">
                <ChatDashboard profileId={params.id} isCompany={true} />
            </div>
        </main>
    );
}