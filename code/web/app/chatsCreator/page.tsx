import { CreatorChatSelection } from '@/components/chatCreator/CreatorChatSelection';

export default async function ChatMessagesPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;

  return (
    <div className="min-h-screen bg-black p-6 md:p-10 flex justify-center items-start">
      <div className="w-full max-w-5xl mt-10">
        <CreatorChatSelection id={id} />
      </div>
    </div>
  );
}

