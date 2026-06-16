'use client';

import { useState } from 'react';
import { LogOut, User as UserIcon, Plus, Check, X } from "lucide-react";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { DropdownMenu, DropdownMenuContent, DropdownMenuGroup, DropdownMenuItem, DropdownMenuLabel, DropdownMenuSeparator, DropdownMenuTrigger,} from "@/components/ui/dropdown-menu";
import { useUser } from "@/providers/UserProvider";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";

export function NavUser() {
    const { user, logout } = useUser();
    const router = useRouter();
    const [isConfirmingLogout, setIsConfirmingLogout] = useState(false);

    if (!user) return null;

    const isCoWorker = user.role === 'coworker';
    const owner = user.ownerInfo;

    const displayName = isCoWorker && owner ? owner.name : user.name;
    const displayEmail = isCoWorker && owner ? owner.email : user.email;
    
    let profileRedirectUrl = `/${user.role}/${user.id}`; 

    if (isCoWorker && owner) {
        profileRedirectUrl = `/${owner.role}/${owner.id}`;
    }

    const handleLogout = async () => {
        await logout();
        router.push('/');
    };

    return (
        <DropdownMenu onOpenChange={(open) => { if (!open) setIsConfirmingLogout(false) }}>
            <DropdownMenuTrigger className="focus:outline-none">
                <Avatar className="h-9 w-9 border-2 border-transparent hover:border-primary transition-all">
                    <AvatarFallback className="bg-primary/10 text-primary text-xs font-bold">
                        {displayName.substring(0, 2).toUpperCase()}
                    </AvatarFallback>
                </Avatar>
            </DropdownMenuTrigger>

            <DropdownMenuContent className="w-64 mt-2 p-2" align="end">
                {!isConfirmingLogout ? (
                    /* --- MENU NORMAL --- */
                    <>
                        <DropdownMenuLabel className="font-normal p-2">
                            <div className="flex flex-col space-y-1">
                                <p className="text-sm font-medium">{displayName}</p>
                                <p className="text-xs text-muted-foreground capitalize">
                                    {isCoWorker ? `coworker (${owner?.role || 'workspace'})` : user.role}
                                </p>
                                <p className="text-[10px] text-muted-foreground/70 truncate">{displayEmail}</p>
                            </div>
                        </DropdownMenuLabel>
                        <DropdownMenuSeparator />
                        <DropdownMenuGroup>
                            
                            {/* Redireciona dinamicamente para o perfil do Owner ou do User comum */}
                            <DropdownMenuItem onClick={() => router.push(profileRedirectUrl)}>
                                <UserIcon className="mr-2 h-4 w-4" />
                                <span>My Profile</span>
                            </DropdownMenuItem>

                            {(user.role === 'creator' || user
                            .ownerInfo?.role === 'creator') && (
                                <DropdownMenuItem onClick={() => router.push('/create-social-profile')}>
                                    <Plus className="mr-2 h-4 w-4" />
                                    <span>Social Profile</span>
                                </DropdownMenuItem>
                            )}

                        </DropdownMenuGroup>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem
                            onClick={(e) => {
                                e.preventDefault();
                                setIsConfirmingLogout(true);
                            }}
                            className="text-destructive focus:bg-destructive/10"
                        >
                            <LogOut className="mr-2 h-4 w-4" />
                            <span>Leave</span>
                        </DropdownMenuItem>
                    </>
                ) : (
                    /* --- MINI PÁGINA DE VALIDAÇÃO (LOGOUT) --- */
                    <div className="flex flex-col gap-3 p-3 text-center animate-in fade-in zoom-in-95 duration-200">
                        <div className="space-y-1">
                            <p className="text-sm font-bold tracking-tight">Do you want to logout?</p>
                            <p className="text-[11px] leading-tight text-muted-foreground">
                                You will need to login again later.
                            </p>
                        </div>

                        <div className="grid grid-cols-2 gap-2">
                            <Button
                                variant="outline"
                                size="sm"
                                className="h-8 text-xs px-2"
                                onClick={(e) => {
                                    e.preventDefault();
                                    setIsConfirmingLogout(false);
                                }}
                            >
                                <X className="mr-1.5 h-3 w-3" />
                                No
                            </Button>
                            <Button
                                variant="destructive"
                                size="sm"
                                className="h-8 text-xs px-2"
                                onClick={handleLogout}
                            >
                                <Check className="mr-1.5 h-3 w-3" />
                                Yes
                            </Button>
                        </div>
                    </div>
                )}
            </DropdownMenuContent>
        </DropdownMenu>
    );
}