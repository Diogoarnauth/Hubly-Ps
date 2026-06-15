'use client';

import React from 'react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { UserCircle, Building2, Users } from 'lucide-react';

type Role = 'creator' | 'company' | 'coworker';

interface RoleSelectionProps {
  onSelect: (role: Role) => void;
}

export function RoleSelection({ onSelect }: RoleSelectionProps) {
  return (
    <Card className="w-full max-w-4xl border-none shadow-none bg-transparent">
      <CardHeader className="text-center">
        <CardTitle className="text-4xl font-bold tracking-tight">
          Welcome to Hubly!
        </CardTitle>
        <CardDescription className="text-xl mt-2">
          You are registering as an...
        </CardDescription>
      </CardHeader>
      <CardContent className="grid grid-cols-1 md:grid-cols-3 gap-6 mt-8">
        <Button variant="outline" className="h-64 flex flex-col gap-6 border-2 hover:border-primary hover:bg-primary/5 transition-all group" onClick={() => onSelect('creator')}>
          <UserCircle size={64} className="group-hover:scale-110 transition-transform text-muted-foreground group-hover:text-primary" />
          <div className="flex flex-col gap-1">
            <span className="text-2xl font-bold text-foreground">Creator</span>
            <span className="text-sm font-normal text-muted-foreground text-wrap">I want to showcase my art</span>
          </div>
        </Button>

        <Button variant="outline" className="h-64 flex flex-col gap-6 border-2 hover:border-primary hover:bg-primary/5 transition-all group" onClick={() => onSelect('company')}>
          <Building2 size={64} className="group-hover:scale-110 transition-transform text-muted-foreground group-hover:text-primary" />
          <div className="flex flex-col gap-1">
            <span className="text-2xl font-bold text-foreground">Company</span>
            <span className="text-sm font-normal text-muted-foreground text-wrap">I am looking for creative talent</span>
          </div>
        </Button>

        <Button variant="outline" className="h-64 flex flex-col gap-6 border-2 hover:border-primary hover:bg-primary/5 transition-all group" onClick={() => onSelect('coworker')}>
          <Users size={64} className="group-hover:scale-110 transition-transform text-muted-foreground group-hover:text-primary" />
          <div className="flex flex-col gap-1">
            <span className="text-2xl font-bold text-foreground">Coworker</span>
            <span className="text-sm font-normal text-muted-foreground text-wrap">I want to assist a creative team</span>
          </div>
        </Button>
      </CardContent>
    </Card>
  );
}