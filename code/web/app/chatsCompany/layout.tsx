import { SignalRProvider } from "@/providers/SignalRContext";

export default function ChatsCompanyLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return <SignalRProvider>{children}</SignalRProvider>;
}