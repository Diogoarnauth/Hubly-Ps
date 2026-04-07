'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { ApiClient } from '@/services/api/apiClient';

export function AuthRedirectHandler() {
  const router = useRouter();

  useEffect(() => {
    const handleUnauthorized = () => {
      router.push('/login');
    };

    ApiClient.setUnauthorizedHandler(handleUnauthorized);
  }, [router]);

  return null;
}