'use client';

import React from 'react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Users, Building2, Search } from 'lucide-react';

interface RoleSelectionSearchProps {
  onSelect: (role: 'creator' | 'company') => void;
}

export function RoleSelectionSearch({ onSelect }: RoleSelectionSearchProps) {
  return (
    <Card className="w-full max-w-4xl mx-auto border-none shadow-none bg-transparent">
      <CardHeader className="text-center space-y-4">
        <div className="flex justify-center">
            <div className="p-3 bg-primary/10 rounded-full text-primary">
                <Search size={32} />
            </div>
        </div>
        <CardTitle className="text-5xl font-black tracking-tighter italic">
          Discovery <span className="text-primary">Hub</span>
        </CardTitle>
        <CardDescription className="text-xl">
          What are you looking for today?
        </CardDescription>
      </CardHeader>
      
      <CardContent className="grid grid-cols-1 md:grid-cols-2 gap-8 mt-12">
        <Button 
          variant="outline" 
          className="h-72 flex flex-col gap-6 border-2 hover:border-primary hover:bg-primary/5 transition-all group rounded-3xl"
          onClick={() => onSelect('creator')}
        >
          <Users size={80} className="group-hover:scale-110 transition-transform text-muted-foreground group-hover:text-primary" />
          <div className="flex flex-col gap-2">
            <span className="text-2xl font-bold text-foreground">Search Creators</span>
            <span className="text-sm font-normal text-muted-foreground">Discover influencers and artists</span>
          </div>
        </Button>

        <Button 
          variant="outline" 
          className="h-72 flex flex-col gap-6 border-2 hover:border-primary hover:bg-primary/5 transition-all group rounded-3xl"
          onClick={() => onSelect('company')}
        >
          <Building2 size={80} className="group-hover:scale-110 transition-transform text-muted-foreground group-hover:text-primary" />
          <div className="flex flex-col gap-2">
            <span className="text-2xl font-bold text-foreground">Search Companies</span>
            <span className="text-sm font-normal text-muted-foreground">Find businesses and opportunities</span>
          </div>
        </Button>
      </CardContent>
    </Card>
  );
}