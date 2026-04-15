'use client';

import React, { useState, useEffect } from 'react';
import companyService from '@/services/api/CompanyService';
import sectorService, { Sector } from '@/services/api/SectorService'; 
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Checkbox } from "@/components/ui/checkbox"; 
import { useRouter } from 'next/navigation';
import { toastSuccess, toastError } from '../ToastImplementations';
import { Loader2 } from 'lucide-react';

export function CompanyForm({ onBack }: { onBack: () => void }) {
  const router = useRouter();
  const [step, setStep] = useState(1);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  
  // Estados para dados da API
  const [availableSectors, setAvailableSectors] = useState<Sector[]>([]);
  const [availableCountries, setAvailableCountries] = useState<string[]>([]);
  const [loadingInitialData, setLoadingInitialData] = useState(true);

  const [formData, setFormData] = useState({
    companySize: 0,
    companyName: '',
    description: '',
    sectors: [] as string[], 
    websiteLink: '',
    countryHeadquarters: ''
  });

  // Carregar setores e países ao montar o componente
  useEffect(() => {
    async function fetchData() {
      try {
        const [sectorsData, countriesData] = await Promise.all([
          sectorService.getAllSectors(),
          companyService.getCountries()
        ]);
        
        setAvailableSectors(sectorsData);
        
        // Limpar e ordenar lista de países (removendo termos genéricos como fizemos na pesquisa)
        const cleanCountries = countriesData
          .filter(c => c !== 'world' && c !== 'Europe')
          .sort((a, b) => a.localeCompare(b));
        
        setAvailableCountries(cleanCountries);
      } catch (err) {
        console.error("Error loading initial data:", err);
      } finally {
        setLoadingInitialData(false);
      }
    }
    fetchData();
  }, []);

  const nextStep = () => setStep((prev) => prev + 1);
  const prevStep = () => {
    if (step === 1) onBack();
    else setStep((prev) => prev - 1);
  };

  const handleSectorChange = (sectorName: string) => {
    setFormData(prev => {
      const isSelected = prev.sectors.includes(sectorName);
      if (isSelected) {
        return { ...prev, sectors: prev.sectors.filter(s => s !== sectorName) };
      } else {
        return { ...prev, sectors: [...prev.sectors, sectorName] };
      }
    });
  };

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    setIsLoading(true);
    setError('');

    try {
      const result = await companyService.registerCompany(formData);

      if (result.success) {
        toastSuccess('Success!', 'Company registered successfully.');
        router.push('/dashboard');
      } else {
        const apiErrorMessage = result.message || 'Failed to register company.';
        setError(apiErrorMessage);
      }
    } catch (err) {
      const fallbackMsg = 'An unexpected error occurred. Please try again later.';
      setError(fallbackMsg);
      toastError('Server Error', fallbackMsg);
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <Card className="border-border bg-card/50 w-full max-w-md mx-auto">
      <CardHeader>
        <CardTitle className="text-2xl">Company Registration</CardTitle>
        <CardDescription>Step {step} of 6</CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-6">
          
          {/* PASSO 1: Tamanho da Empresa */}
          {step === 1 && (
            <div className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="companySize">How many employees does your company have?</Label>
                <Input 
                  id="companySize"
                  type="number" 
                  value={formData.companySize || ''} 
                  onChange={(e) => setFormData({...formData, companySize: parseInt(e.target.value) || 0})}
                  placeholder="Ex: 50"
                  required
                />
              </div>
              <Button type="button" className="w-full" onClick={nextStep} disabled={formData.companySize <= 0}>Next</Button>
            </div>
          )}

          {/* PASSO 2: Nome da Empresa */}
          {step === 2 && (
            <div className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="companyName">What is the company name?</Label>
                <Input 
                  id="companyName"
                  value={formData.companyName} 
                  onChange={(e) => setFormData({...formData, companyName: e.target.value})}
                  placeholder="Enter company name"
                  required
                />
              </div>
              <Button type="button" className="w-full" onClick={nextStep} disabled={!formData.companyName}>Next</Button>
            </div>
          )}

          {/* PASSO 3: Descrição */}
          {step === 3 && (
            <div className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="description">Brief description of the company</Label>
                <Input 
                  id="description"
                  value={formData.description} 
                  onChange={(e) => setFormData({...formData, description: e.target.value})}
                  placeholder="What do you do?"
                  required
                />
              </div>
              <Button type="button" className="w-full" onClick={nextStep} disabled={!formData.description}>Next</Button>
            </div>
          )}

          {/* PASSO 4: Setores */}
          {step === 4 && (
            <div className="space-y-4">
              <Label className="text-base">Select the business sectors:</Label>
              {loadingInitialData ? (
                <div className="flex flex-col items-center py-4 gap-2">
                  <Loader2 className="animate-spin text-primary" size={20} />
                  <span className="text-sm text-muted-foreground">Loading sectors...</span>
                </div>
              ) : (
                <div className="grid grid-cols-1 gap-2 max-h-48 overflow-y-auto p-3 border rounded-md bg-background/50">
                  {availableSectors.map((sector) => (
                    <div key={sector.id} className="flex items-center space-x-3 py-1">
                      <Checkbox 
                        id={`sector-${sector.id}`} 
                        checked={formData.sectors.includes(sector.name)}
                        onCheckedChange={() => handleSectorChange(sector.name)}
                      />
                      <Label 
                        htmlFor={`sector-${sector.id}`} 
                        className="text-sm font-normal cursor-pointer flex-1"
                      >
                        {sector.name}
                      </Label>
                    </div>
                  ))}
                </div>
              )}
              <Button 
                type="button" 
                className="w-full" 
                onClick={nextStep} 
                disabled={formData.sectors.length === 0}
              >
                Next ({formData.sectors.length} selected)
              </Button>
            </div>
          )}

          {/* PASSO 5: Website */}
          {step === 5 && (
            <div className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="website">Website Link</Label>
                <Input 
                  id="website"
                  type="url"
                  value={formData.websiteLink} 
                  onChange={(e) => setFormData({...formData, websiteLink: e.target.value})}
                  placeholder="https://www.company.com"
                  required
                />
              </div>
              <Button type="button" className="w-full" onClick={nextStep} disabled={!formData.websiteLink}>Next</Button>
            </div>
          )}

          {/* PASSO 6: Sede / País (Dropdown Dinâmica) */}
          {step === 6 && (
            <div className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="country">Country Headquarters</Label>
                {loadingInitialData ? (
                  <div className="flex items-center gap-2 py-2">
                    <Loader2 className="animate-spin text-primary" size={16} />
                    <span className="text-xs text-muted-foreground">Loading countries...</span>
                  </div>
                ) : (
                  <select
                    id="country"
                    className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
                    value={formData.countryHeadquarters}
                    onChange={(e) => setFormData({...formData, countryHeadquarters: e.target.value})}
                    required
                  >
                    <option value="" disabled>Select a country</option>
                    {availableCountries.map(country => (
                      <option key={country} value={country} className="bg-slate-900">
                        {country}
                      </option>
                    ))}
                  </select>
                )}
              </div>
              <Button type="submit" className="w-full" disabled={isLoading || !formData.countryHeadquarters}>
                {isLoading ? <Loader2 className="animate-spin mr-2" size={18} /> : null}
                {isLoading ? 'Registering...' : 'Complete Registration'}
              </Button>
            </div>
          )}

          {/* Navegação Secundária */}
          <div className="pt-2">
            <Button 
              type="button" 
              variant="ghost" 
              className="w-full text-muted-foreground" 
              onClick={prevStep} 
              disabled={isLoading}
            >
              ← Back
            </Button>
          </div>

          {error && (
            <Alert variant="destructive" className="mt-4">
              <AlertDescription>{error}</AlertDescription>
            </Alert>
          )}
        </form>
      </CardContent>
    </Card>
  );
}