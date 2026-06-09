'use client';

import { useEffect } from 'react';
import { usePathname, useRouter } from 'next/navigation';
import { ApiClient } from '@/services/api/apiClient';

export function AuthRedirectHandler() {
  const router = useRouter();
  const pathname = usePathname();

  useEffect(() => {
    const handleUnauthorized = () => {
      if (pathname !== '/') {
        router.push('/login');
      }
    };

    ApiClient.setUnauthorizedHandler(handleUnauthorized);
  }, [pathname, router]);

  return null;
}