import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

const publicRoutes = ['/', '/register', '/register/confirmEmail'];

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;
  
  const isPublicRoute = publicRoutes.some(route => pathname === route);
  
  const token = request.cookies.get('auth_token')?.value;
  
  if (!isPublicRoute && !token) {
    return NextResponse.redirect(new URL('/', request.url));
  }
  
  if ((pathname === '/' || pathname === '/register') && token) {
    return NextResponse.redirect(new URL('/dashboard', request.url));
  }
  
  return NextResponse.next();
}

export const config = {
  matcher: ['/((?!api|_next|.*\\..*).*)'],
};

