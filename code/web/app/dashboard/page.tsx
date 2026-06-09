'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useUser } from '@/providers/UserProvider';
import { Dashboard } from '@/components/dashboard/Dashboard';

export default function DashboardPage() {
  const router = useRouter();
  const { user, loading, refreshUser } = useUser();
  const [isChecking, setIsChecking] = useState(true);

  useEffect(() => {
    if (loading) return;

    const validateAccess = async () => {

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
  }, [user, loading, refreshUser]);

  useEffect(() => {
    if (!isChecking && !loading) {
      if (!user || user.role === null) {
        router.replace('/onboarding');
      }
    }
  }, [isChecking, loading, user, router]);

  if (loading || isChecking) {
    return (
      <div className="flex h-svh w-full items-center justify-center">
        <div className="animate-pulse text-muted-foreground font-medium">
          Sincronization in progress...
        </div>
      </div>
    );
  }

  return user?.role ? <Dashboard /> : null;
}