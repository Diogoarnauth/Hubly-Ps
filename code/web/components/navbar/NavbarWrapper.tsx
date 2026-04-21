'use client';

import { usePathname } from 'next/navigation';
import { Navbar } from './Navbar';

export function NavbarWrapper() {
  const pathname = usePathname();

  // Define aqui as páginas onde a Navbar NÃO deve aparecer
  const blackList = [
    '/login',
    '/register',
    '/onboarding',
    '/', 
  ];

  if (blackList.includes(pathname)) {
    return null;
  }

  return <Navbar />;
}