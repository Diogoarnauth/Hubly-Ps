import type { Metadata } from "next";
import { Toaster } from "@/components/ui/sonner"
import "./globals.css";
import { ThemeProvider } from "@/providers/ThemeProvider"; 
import { AuthRedirectHandler } from "@/components/AuthRedirectHandler";

export const metadata: Metadata = {
  title: "Hubly",
  description: "An Hub where Creators and Company meet",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body>
        <ThemeProvider
          attribute="class"
          defaultTheme="system"
          enableSystem
          disableTransitionOnChange
        >
          <Toaster/>
          <AuthRedirectHandler />
          {children}
        </ThemeProvider>
      </body>
    </html>
  );
}
