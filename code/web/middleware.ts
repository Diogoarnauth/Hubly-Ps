import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';
import { publicRoutes } from '@/lib/publicRoutes';

export function middleware(request: NextRequest) {

  const { pathname } = request.nextUrl;
  
  const isPublicRoute = publicRoutes.some(route => pathname === route);
  
  const token = request.cookies.get('token')?.value;

  console.log("Middleware executado para:", pathname);
  
  if (!isPublicRoute && !token && !(pathname === '/')) {
    console.log("entrei aqui")
    return NextResponse.redirect(new URL('/login', request.url));
  }
  
  if ((pathname === '/login' || pathname === '/register' || pathname === '/register/confirmEmail') && token) {
    return NextResponse.redirect(new URL('/', request.url));
  }

  console.log("Middleware passou ");

  
  return NextResponse.next();
}

export const config = {
  matcher: ['/((?!api|_next|.*\\..*).*)'],
};

