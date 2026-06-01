import { Metadata } from "next";
import { Providers } from "../providers/Providers";
import { Toaster } from "@/components/ui/sonner";
import { AuthRedirectHandler } from "@/components/AuthRedirectHandler";
import { NavbarWrapper } from "@/components/navbar/NavbarWrapper";
import "./globals.css";

export const metadata: Metadata = { title: "Hubly", description: "..." };

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body>
        <Providers>
          <Toaster />
          <AuthRedirectHandler />
          <NavbarWrapper />
          <main className="relative">{children}</main>
        </Providers>
      </body>
    </html>
  );
}