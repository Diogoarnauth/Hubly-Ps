'use client';

import { usePathname, useRouter } from "next/navigation";
import { Home, Search } from "lucide-react";
import { NavUser } from "./nav-user";
import { useUser } from "@/providers/UserProvider";

export function Navbar() {
  const pathname = usePathname();
  const router = useRouter();
  const { user } = useUser();

  if (!user) return null;

  const navItems = [
    { title: "Home", url: "/dashboard", icon: Home },
    { title: "Pesquisar", url: "/search", icon: Search },
  ];

  return (
    <nav className="fixed top-0 w-full h-16 border-b bg-background/95 backdrop-blur z-50">
      <div className="max-w-7xl mx-auto px-6 h-full flex items-center justify-between">
        
        {/* LADO ESQUERDO: Logo e Links */}
        <div className="flex items-center gap-10">
          <span 
            onClick={() => router.push('/dashboard')}
            className="text-xl font-black tracking-tighter text-primary cursor-pointer"
          >
            HUBLY
          </span>
          
          <div className="flex items-center gap-6">
            {navItems.map((item) => {
              const isActive = pathname === item.url;
              return (
                <button
                  key={item.url}
                  onClick={() => router.push(item.url)}
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

        {/* LADO DIREITO: Perfil e Logout */}
        <div className="flex items-center gap-4">
          <NavUser />
        </div>
      </div>
    </nav>
  );
}