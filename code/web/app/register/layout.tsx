import { RegisterProvider } from "@/providers/RegisterContext";

export default function RegisterLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <RegisterProvider>
        {children}
    </RegisterProvider>
  );
}