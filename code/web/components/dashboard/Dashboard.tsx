'use client';

import React from 'react';
import { CreatorCarousel } from './CreatorCarousel';
import { CompanyCarousel } from './CompanyCarousel';
import { CreatorRecommendationFeed } from './CreatorRecommendationFeed';
import { CompanyRecommendationFeed } from './CompanyRecommendationFeed';
import { Sparkles, Trophy, Rocket } from "lucide-react";

export function Dashboard() {
  return (
    <div className="flex flex-col gap-20 py-16 px-4 md:px-8 max-w-[1400px] mx-auto">
      
      {/* Hero Section - Mais elegante e centralizada */}
      <section className="text-center space-y-6 max-w-4xl mx-auto mb-8">
        <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-primary/10 text-primary text-xs font-bold uppercase tracking-widest mb-4 animate-fade-in">
          <Sparkles size={14} />
          <span>The Future of Collaboration</span>
        </div>
        
        <h1 className="text-5xl md:text-7xl font-black tracking-tighter italic leading-[0.9]">
          Welcome to <span className="text-primary drop-shadow-sm">Hubly</span>
        </h1>
        
        <p className="text-xl md:text-2xl text-muted-foreground leading-relaxed font-light max-w-2xl mx-auto">
          The elite ecosystem that <span className="text-foreground font-medium underline decoration-primary/30 underline-offset-4">bridges</span> creative masters with visionary businesses.
        </p>
      </section>

      <div className="space-y-24">
        {/* Seção de Creators */}
        <section className="relative">
          <div className="flex flex-col items-center text-center mb-10 space-y-2">
            <div className="p-2 bg-secondary/50 rounded-xl mb-2">
              <Trophy className="text-primary" size={24} />
            </div>
            <h2 className="text-3xl font-extrabold tracking-tight md:text-4xl">
              Trending Creators
            </h2>
            <p className="text-muted-foreground max-w-md">
              Discover the digital artists and influencers who are capturing the world&apos;s attention this week.
            </p>
            <div className="h-1 w-20 bg-primary/20 rounded-full mt-4" />
          </div>
          
          <div className="relative z-10">
            <CreatorCarousel />
          </div>

          <div className="mt-10">
            <div className="flex flex-col items-center text-center mb-8 space-y-2">
              <h3 className="text-2xl font-bold tracking-tight md:text-3xl">Recommended Creators</h3>
              <p className="text-muted-foreground max-w-2xl">
                Creative matches selected for you based on recent activity and fit.
              </p>
            </div>
            <CreatorRecommendationFeed />
          </div>
        </section>

        <hr className="border-border/50 max-w-4xl mx-auto" />

        {/* Seção de Companies */}
        <section className="relative">
          <div className="flex flex-col items-center text-center mb-10 space-y-2">
            <div className="p-2 bg-secondary/50 rounded-xl mb-2">
              <Rocket className="text-primary" size={24} />
            </div>
            <h2 className="text-3xl font-extrabold tracking-tight md:text-4xl">
              Top Tier Companies
            </h2>
            <p className="text-muted-foreground max-w-md">
              Leading brands and startups actively searching for their next big creative partnership.
            </p>
            <div className="h-1 w-20 bg-primary/20 rounded-full mt-4" />
          </div>

          <div className="relative z-10">
            <CompanyCarousel />
          </div>

          <div className="mt-10">
            <div className="flex flex-col items-center text-center mb-8 space-y-2">
              <h3 className="text-2xl font-bold tracking-tight md:text-3xl">Recommended Companies</h3>
              <p className="text-muted-foreground max-w-2xl">
                Curated company suggestions based on your creative interests and project goals.
              </p>
            </div>
            <CompanyRecommendationFeed />
          </div>
        </section>
      </div>

      {/* Footer subtil ou frase de fecho */}
      <section className="text-center pt-10 border-t border-border/40">
        <p className="text-sm text-muted-foreground italic">
          Powering over 500+ successful connections daily.
        </p>
      </section>
    </div>
  );
}