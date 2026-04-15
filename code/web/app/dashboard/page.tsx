'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import usersService from '@/services/api/UsersService';
import { Dashboard } from '@/components/dashboard/Dashboard';

export default function DashboardPage() {
  const router = useRouter();
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function checkAccess() {
      const user = await usersService.checkHasProfile();

      if (!user) {
        //router.replace('/onboarding');
        return;
      }

      setLoading(false);
    }
    checkAccess();
  }, [router]);

  if (loading) {
    return (
      <div className="flex h-svh w-full items-center justify-center">
        <div className="animate-pulse text-muted-foreground">Loading Hubly...</div>
      </div>
    );
  }

  return <Dashboard />;
}