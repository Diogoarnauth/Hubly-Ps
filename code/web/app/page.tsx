'use client';

import { useEffect, useState } from 'react';
import { usePathname, useRouter } from 'next/navigation';
import { useUser } from '@/providers/UserProvider';
import { Dashboard } from '@/components/dashboard/Dashboard';
import { publicRoutes } from '@/lib/publicRoutes';

export default function DashboardPage() {
  const router = useRouter();
  const pathname = usePathname();
  const isPublicRoute = publicRoutes.includes(pathname);
  const { user, loading, refreshUser } = useUser();
  const [isChecking, setIsChecking] = useState(true);

  useEffect(() => {
    if (loading) return;

    const validateAccess = async () => {
      if (isPublicRoute) {
        setIsChecking(false);
        return;
      }

      if (user && user.role !== null) {
        setIsChecking(false);
        return;
      }

      try {
        await refreshUser();
      } catch (error) {
        console.error("Erro ao validar acesso na Dashboard:", error);
      } finally {
        setIsChecking(false);
      }
    };

    validateAccess();
  }, [user, loading, refreshUser, isPublicRoute]);

  useEffect(() => {
    if (!isChecking && !loading && !isPublicRoute) {
      if (!user || user.role === null) {
        router.replace('/onboarding');
      }
    }
  }, [isChecking, loading, user, router, isPublicRoute]);

  if (loading || isChecking) {
    return (
      <div className="flex h-svh w-full items-center justify-center">
        <div className="animate-pulse text-muted-foreground font-medium">
          Sincronization in progress...
        </div>
      </div>
    );
  }

  if (isPublicRoute) {
    return <Dashboard />;
  }

  return user?.role ? <Dashboard /> : null;
}