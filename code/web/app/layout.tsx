import type { Metadata } from "next";
import { Toaster } from "@/components/ui/sonner"
import "./globals.css";
import { ThemeProvider } from "@/providers/ThemeProvider"; 
import { UserProvider } from "@/providers/UserProvider";
import { AuthRedirectHandler } from "@/components/AuthRedirectHandler";
import { NavbarWrapper } from "@/components/navbar/NavbarWrapper"; 

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
    <html lang="en" suppressHydrationWarning>
      <body>
        <ThemeProvider
          attribute="class"
          defaultTheme="system"
          enableSystem
          disableTransitionOnChange
        >
          <UserProvider>
            <Toaster />
            <AuthRedirectHandler />
            
            {/* Smart NavBar*/}
            <NavbarWrapper />

            <div className="relative">
               {children}
            </div>
          </UserProvider>
        </ThemeProvider>
      </body>
    </html>
  );
}