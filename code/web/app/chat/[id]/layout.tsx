import { SignalRProvider } from "@/providers/SignalRContext";

export default function ChatMessagesLayout({
    children,
}: Readonly<{
    children: React.ReactNode;
}>) {
    return (
        <SignalRProvider>
            {children}
        </SignalRProvider>
    );
}