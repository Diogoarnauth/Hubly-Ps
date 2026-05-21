'use client';

import React, { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import sectorService, { Sector } from '@/services/api/SectorService';
import creatorService from '@/services/api/CreatorService'; 
import platformService, { Platform } from '@/services/api/PlatformService'; 
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea"; // Importação adicionada aqui
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Checkbox } from "@/components/ui/checkbox";
import { toastSuccess, toastError } from '../ToastImplementations';
import { Loader2, SearchIcon, Check } from 'lucide-react';
import { cn } from "@/lib/utils";

export function SocialProfileForm() {
  const router = useRouter();
  const [step, setStep] = useState(1);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  
  const [availableSectors, setAvailableSectors] = useState<Sector[]>([]);
  const [availablePlatforms, setAvailablePlatforms] = useState<Platform[]>([]);
  const [loadingData, setLoadingData] = useState(true);
  const [sectorSearch, setSectorSearch] = useState('');

  const [formData, setFormData] = useState({
    platform_user_name: '',
    link: '',
    description: '',
    followers_count: 0,
    priceMin: undefined as number | undefined,
    priceMax: undefined as number | undefined,
    platformId: 0,
    sectors: [] as string[]
  });

  useEffect(() => {
    async function fetchData() {
      try {
        const [sectorsData, platformsData] = await Promise.all([
          sectorService.getAllSectors(),
          platformService.getAllPlatforms()
        ]);
        setAvailableSectors(sectorsData);
        setAvailablePlatforms(platformsData);
        console.log("platformsData", platformsData)
      } catch (err) {
        console.error("Error loading form data:", err);
      } finally {
        setLoadingData(false);
      }
    }
    fetchData();
  }, []);

  const nextStep = () => setStep((p) => p + 1);
  const prevStep = () => setStep((p) => p - 1);

  const handleSectorChange = (sectorName: string) => {
    setFormData(prev => ({
      ...prev,
      sectors: prev.sectors.includes(sectorName)
        ? prev.sectors.filter(s => s !== sectorName)
        : [...prev.sectors, sectorName]
    }));
  };

  const filteredSectors = availableSectors.filter(s => 
    s.name.toLowerCase().includes(sectorSearch.toLowerCase())
  );

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    setIsLoading(true);
    setError('');

    try {
      const result = await creatorService.addSocialProfile(formData);
      console.log("result", result)
      console.log("result.message", result.data.message != null)
      if (result.data.message == null) {
        toastSuccess('Success!', 'Social profile added.');
        router.push(`/creator/${result.data.creatorId}`);
      } else {
        setError(result.data.message || 'Error adding profile.');
      }
    } catch (err) {
      setError('Server connection failed.');
      toastError('Error', 'Could not save profile.');
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <Card className="border-border bg-card/50 w-full max-w-md mx-auto shadow-xl">
      <CardHeader>
        <div className="flex justify-between items-center">
          <CardTitle className="text-xl italic font-bold text-primary">New Social Profile</CardTitle>
          <span className="text-xs text-muted-foreground font-mono">Step {step}/6</span>
        </div>
        <CardDescription>Fill in the details for your new platform.</CardDescription>
      </CardHeader>
      
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-6">
          
          {/* PASSO 1: Seleção de Plataforma */}
          {step === 1 && (
            <div className="space-y-4 animate-in fade-in slide-in-from-right-2">
              <Label className="text-xs font-bold uppercase tracking-widest text-primary">Select Platform</Label>
              {loadingData ? (
                <div className="flex justify-center py-4"><Loader2 className="animate-spin text-primary" size={20} /></div>
              ) : (
                <div className="grid grid-cols-2 gap-2">
                  {availablePlatforms.map((p) => (
                    <Button
                      key={p.id}
                      type="button"
                      variant={formData.platformId === p.id ? "default" : "outline"}
                      className={cn(
                        "h-12 justify-center font-bold transition-all", 
                        formData.platformId === p.id && "bg-primary text-primary-foreground scale-[1.02]"
                      )}
                      onClick={() => { setFormData({...formData, platformId: p.id}); nextStep(); }}
                    >
                      {p.namePlatform}
                    </Button>
                  ))}
                </div>
              )}
            </div>
          )}

          {/* PASSO 2: Identificação */}
          {step === 2 && (
            <div className="space-y-4 animate-in fade-in slide-in-from-right-2">
              <div className="space-y-2">
                <Label htmlFor="username">Platform Username (@)</Label>
                <Input 
                  id="username" 
                  value={formData.platform_user_name} 
                  onChange={e => setFormData({...formData, platform_user_name: e.target.value})}
                  placeholder="ex: johndoe_official" 
                  required
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="link">Profile URL Link</Label>
                <Input 
                  id="link" 
                  type="url" 
                  value={formData.link} 
                  onChange={e => setFormData({...formData, link: e.target.value})}
                  placeholder="https://instagram.com/johndoe" 
                  required
                />
              </div>
              <Button type="button" className="w-full" onClick={nextStep} disabled={!formData.platform_user_name || !formData.link}>Next</Button>
            </div>
          )}

          {/* PASSO 3: Descrição Modificada para Textarea Expandido */}
          {step === 3 && (
            <div className="space-y-4 animate-in fade-in slide-in-from-right-2">
              <div className="space-y-2">
                <Label htmlFor="description">Profile Description</Label>
                <Textarea 
                  id="description"
                  value={formData.description} 
                  onChange={(e) => setFormData({...formData, description: e.target.value})}
                  rows={6}
                  className="resize-none min-h-[150px] bg-background/50 text-sm placeholder:text-muted-foreground/70"
                  placeholder={
                    "Tell us about your content:\n\n" +
                    "• Your Rates / Media Kit (e.g., Price per Reel, Post, Story, UGC)\n" +
                    "• Audience Demographics (e.g., Main countries, Age groups, Gender)\n" +
                    "• Types of Partnerships open to (e.g., Paid campaigns, Affiliate, Gifting)"
                  }
                  required
                />
              </div>
              <Button type="button" className="w-full" onClick={nextStep} disabled={!formData.description}>Next</Button>
            </div>
          )}

          {/* PASSO 4: Métricas e Preços */}
          {step === 4 && (
            <div className="space-y-4 animate-in fade-in slide-in-from-right-2">
              <div className="space-y-2">
                <Label htmlFor="followers">Followers Count</Label>
                <Input 
                  id="followers" 
                  type="number" 
                  value={formData.followers_count || ''} 
                  onChange={e => setFormData({...formData, followers_count: parseInt(e.target.value) || 0})}
                  placeholder="Ex: 15000"
                />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label>Min Price (€)</Label>
                  <Input 
                    type="number" 
                    placeholder="0" 
                    value={formData.priceMin ?? ''}
                    onChange={e => setFormData({...formData, priceMin: e.target.value ? parseFloat(e.target.value) : undefined})} 
                  />
                </div>
                <div className="space-y-2">
                  <Label>Max Price (€)</Label>
                  <Input 
                    type="number" 
                    placeholder="500" 
                    value={formData.priceMax ?? ''}
                    onChange={e => setFormData({...formData, priceMax: e.target.value ? parseFloat(e.target.value) : undefined})} 
                  />
                </div>
              </div>
              <Button type="button" className="w-full" onClick={nextStep} disabled={formData.followers_count <= 0}>Next</Button>
            </div>
          )}

          {/* PASSO 5: Setores */}
          {step === 5 && (
            <div className="space-y-4 animate-in fade-in slide-in-from-right-2">
              <Label className="text-xs font-bold uppercase tracking-widest text-primary">Content Sectors</Label>
              <div className="relative mb-2">
                <SearchIcon className="absolute left-2 top-1/2 -translate-y-1/2 text-muted-foreground" size={12} />
                <input
                  type="text"
                  placeholder="Search sectors..."
                  className="w-full bg-white/5 border border-white/10 rounded px-7 py-2 text-[10px] text-white outline-none focus:border-primary/50 transition-all"
                  value={sectorSearch}
                  onChange={e => setSectorSearch(e.target.value)}
                />
              </div>
              <div className="max-h-40 overflow-y-auto space-y-1 pr-2 custom-scrollbar border border-white/5 p-2 rounded-lg bg-black/20">
                {filteredSectors.map(s => (
                  <div 
                    key={s.id} 
                    onClick={() => handleSectorChange(s.name)}
                    className={cn(
                      "flex items-center justify-between px-3 py-2 rounded-md cursor-pointer transition-all group",
                      formData.sectors.includes(s.name) ? "bg-primary/20 border border-primary/30" : "hover:bg-white/5 border border-transparent"
                    )}
                  >
                    <span className={cn(
                      "text-sm transition-colors",
                      formData.sectors.includes(s.name) ? "text-primary font-bold" : "text-white/70 group-hover:text-white"
                    )}>
                      {s.name}
                    </span>
                    {formData.sectors.includes(s.name) && <Check size={14} className="text-primary" />}
                  </div>
                ))}
              </div>
              <Button type="button" className="w-full" onClick={nextStep} disabled={formData.sectors.length === 0}>Review</Button>
            </div>
          )}

          {/* PASSO 6: Review Final */}
          {step === 6 && (
            <div className="space-y-4 animate-in fade-in slide-in-from-bottom-2">
              <div className="rounded-lg bg-primary/5 p-4 border border-primary/20 space-y-3">
                <h4 className="text-xs font-bold uppercase text-primary border-b border-primary/10 pb-1">Summary</h4>
                <div className="grid grid-cols-2 gap-y-2 text-sm">
                  <span className="text-muted-foreground">Platform:</span>
                  <span className="text-right">{availablePlatforms.find(p => p.id === formData.platformId)?.namePlatform}</span>
                  <span className="text-muted-foreground">User:</span>
                  <span className="text-right font-mono text-xs">{formData.platform_user_name}</span>
                  <span className="text-muted-foreground">Followers:</span>
                  <span className="text-right">{formData.followers_count.toLocaleString()}</span>
                  <span className="text-muted-foreground">Sectors:</span>
                  <span className="text-right text-xs">{formData.sectors.join(', ')}</span>
                </div>
              </div>
              <Button type="submit" className="w-full shadow-lg shadow-primary/20" disabled={isLoading}>
                {isLoading ? <Loader2 className="animate-spin mr-2" size={18} /> : null}
                {isLoading ? 'Creating...' : 'Confirm & Create Profile'}
              </Button>
            </div>
          )}

          {/* Botão de Voltar Geral */}
          {step > 1 && (
            <Button type="button" variant="ghost" className="w-full text-muted-foreground hover:text-white" onClick={prevStep} disabled={isLoading}>
              ← Back
            </Button>
          )}

          {error && (
            <Alert variant="destructive" className="mt-4 border-red-500/50 bg-red-500/10">
              <AlertDescription>{error}</AlertDescription>
            </Alert>
          )}
        </form>
      </CardContent>
    </Card>
  );
}