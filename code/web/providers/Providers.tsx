'use client';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useState } from 'react';
import { UserProvider } from "@/providers/UserProvider";
import { ThemeProvider } from "@/providers/ThemeProvider";

export function Providers({ children }: { children: React.ReactNode }) {
  const [queryClient] = useState(() => new QueryClient());
  return (
    <QueryClientProvider client={queryClient}>
      <ThemeProvider attribute="class" defaultTheme="system" enableSystem disableTransitionOnChange>
        <UserProvider>{children}</UserProvider>
      </ThemeProvider>
    </QueryClientProvider>
  );
}