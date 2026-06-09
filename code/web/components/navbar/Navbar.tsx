'use client';

import { usePathname, useRouter } from "next/navigation";
import { Home, Search, MessageSquare, LogIn } from "lucide-react";
import { NavUser } from "./nav-user";
import { useUser } from "@/providers/UserProvider";

export function Navbar() {
  const pathname = usePathname();
  const router = useRouter();
  const { user } = useUser();

  const navItems = [
    { title: "Home", url: "/dashboard", icon: Home },
    { title: "Search", url: "/search", icon: Search },
    { title: "Messages", icon: MessageSquare },
  ];

  const handleMessagesClick = () => {
    const userData = user as any;

    if (userData.role === 'creator') {
      router.push('/chatsCreator');
    } else if (userData.role === 'company') {
      const idParaNavegar = userData.companyId || userData.id;
      
      if (idParaNavegar) {
        router.push(`/chatsCompany/${idParaNavegar}`);
      } else {
        console.error("Hubly: User data is missing companyId for company role or id for creator role.");
      }
    }
  };

  return (
    <nav className="fixed top-0 w-full h-16 border-b bg-background/95 backdrop-blur z-50">
      <div className="max-w-7xl mx-auto px-6 h-full flex items-center justify-between">
        
        {/* LADO ESQUERDO: Logo e Links */}
        <div className="flex items-center gap-10">
          <span 
            onClick={() => router.push('/')}
            className="text-xl font-black tracking-tighter text-primary cursor-pointer"
          >
            HUBLY
          </span>
          
          {/* Mostrar navItems apenas quando user está autenticado */}
          { (
            <div className="flex items-center gap-6">
              {navItems.map((item) => {
                const isMessages = item.title === "Messages";
                const isActive = isMessages 
                  ? (pathname.includes("chatsCreator") || pathname.includes("chatsCompany"))
                  : pathname === item.url;

                return (
                  <button
                    key={item.title}
                    onClick={() => isMessages ? handleMessagesClick() : router.push(item.url!)}
                    className={`flex items-center gap-2 text-sm font-medium transition-colors ${
                      isActive ? "text-primary" : "text-muted-foreground hover:text-foreground"
                    }`}
                  >
                    <item.icon className="h-4 w-4" />
                    {item.title}
                  </button>
                );
              })}
            </div>
          )}
        </div>

        {/* LADO DIREITO: Perfil, Logout ou Login */}
        <div className="flex items-center gap-4">
          {user ? (
            <NavUser />
          ) : (
            <button
              onClick={() => router.push('/login')}
              className="flex items-center gap-2 px-4 py-2 text-sm font-medium rounded-md bg-primary text-primary-foreground hover:bg-primary/90 transition-colors"
            >
              <LogIn className="h-4 w-4" />
              Login
            </button>
          )}
        </div>
      </div>
    </nav>
  );
}
