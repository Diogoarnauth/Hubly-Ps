import { SignalRProvider } from "@/providers/SignalRContext";

export default function ChatsCreatorLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return <SignalRProvider>{children}</SignalRProvider>;
}   