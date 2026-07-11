'use client';

import { usePathname, useRouter } from "next/navigation";
import { Home, Search, MessageSquare, LogIn, UserPlus } from "lucide-react";
import { NavUser } from "./nav-user";
import { useUser } from "@/providers/UserProvider";

export function Navbar() {
  const pathname = usePathname();
  const router = useRouter();
  const { user, loading } = useUser();

  const navItems = [
    { title: "Home", url: "/", icon: Home },
    { title: "Search", url: "/search", icon: Search },
    { title: "Messages", icon: MessageSquare },
    { title: "Invites", url: "/invite", icon: UserPlus }, 
  ];

  const handleMessagesClick = () => {
    const userData = user as any;

    if(!userData) {
      router.push('/login');
      return;
    }

    // 1. Fluxo Direto: Creator
    if (userData.role === 'creator') {
      router.push('/chatsCreator/');
      return;
    } 
    
    // 2. Fluxo Direto: Company
    if (userData.role === 'company') {
      const idParaNavegar = userData.companyId || userData.id;
      if (idParaNavegar) {
        router.push(`/chatsCompany/${idParaNavegar}`);
      }
      return;
    } 

    // 3. Fluxo Dinâmico: CoWorker (Verifica a role do Owner)
    if (userData.role === 'coworker') {
      const owner = userData.ownerInfo;
      
      if (!owner) {
        console.error("Hubly: Owner info missing for coworker.");
        return;
      }

      if (owner.role === 'creator') {
        router.push('/chatsCreator');
      } else if (owner.role === 'company') {
        router.push(`/chatsCompany/${owner.id}`);
      } else {
        console.error(`Hubly: Unknown owner role (${owner.role}) for coworker.`);
      }
    }
  };

  return (
    <nav className="fixed top-0 w-full h-16 border-b bg-background/95 backdrop-blur z-50">
      <div className="max-w-7xl mx-auto px-6 h-full flex items-center justify-between">
        
        <div className="flex items-center gap-10">
          <span onClick={() => router.push('/')} className="text-xl font-black tracking-tighter text-primary cursor-pointer">
            HUBLY
          </span>
          
          <div className="flex items-center gap-6">
            {navItems
              .filter(item => {
                // Filtro: Esconder "Invites" se não estiver logado
                if (item.title === "Invites" && !user) return false;
                return true;
              })
              .map((item) => {
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
        </div>

        {/* LADO DIREITO: Perfil, Logout ou Login */}
        <div className="flex items-center gap-4">
          {loading ? (
            <div className="h-9 w-24 rounded-md bg-zinc-700 animate-pulse" />
          ) : user ? (
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