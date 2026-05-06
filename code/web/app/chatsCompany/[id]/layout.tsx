import { SignalRProvider } from "@/providers/SignalRContext";

export default function ChatCompanyMessagesLayout({
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