'use client';

import React, { useState, useEffect, useMemo } from 'react';
import { Search, Filter, ArrowLeft, Loader2, Building2, Globe, SearchIcon } from 'lucide-react';
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { Checkbox } from "@/components/ui/checkbox";
import companyService from '@/services/api/CompanyService';
import sectorService, { Sector } from '@/services/api/SectorService';
import { useRouter } from 'next/navigation';


const SIZE_OPTIONS = [
    "0 a 100",
    "100 a 1000",
    "1000 a 10000",
    "10000 a 100000",
    "100000 a 1000000",
    "+1M"
];

export function CompanySearch({ onBack }: { onBack: () => void }) {
    const [showFilters, setShowFilters] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [results, setResults] = useState<any>(null);
    const [sectors, setSectors] = useState<Sector[]>([]);
    const [allCountries, setAllCountries] = useState<string[]>([]);
    const [countrySearch, setCountrySearch] = useState('');
    const router = useRouter();
    

    const [filters, setFilters] = useState({
        name: '',
        companySize: '',
        selectedSectors: [] as string[],
        selectedCountries: [] as string[],
        page: 1,
        pageSize: 10
    });

    // Carregar dados iniciais (Setores e Países vindos do Domínio/API)
    useEffect(() => {
        const loadMetadata = async () => {
            try {
                const [sectorData, countries] = await Promise.all([
                    sectorService.getAllSectors(),
                    companyService.getCountries()
                ]);

                setSectors(sectorData);

                const cleanList = countries
                    .filter(c => c !== 'world' && c !== 'Europe')
                    .sort((a, b) => a.localeCompare(b));
                setAllCountries(cleanList);
                console.log("cleanList", cleanList);
            } catch (error) {
                console.error(error);
            }
        };

        loadMetadata();
    }, []);

    // Monitoriza a mudança de página para disparar a pesquisa automaticamente
    useEffect(() => {
        if (results) {
            handleSearch();
        }
    }, [filters.page]);

    // Filtro visual para a lista de checkboxes de países
    const filteredCountriesList = useMemo(() => {
        return allCountries.filter(c =>
            c.toLowerCase().includes(countrySearch.toLowerCase())
        );
    }, [countrySearch, allCountries]);

    const handleSearch = async () => {
        setIsLoading(true);
        try {
            const response = await companyService.search({
                name: filters.name || undefined,
                companySize: filters.companySize || undefined,
                sectors: filters.selectedSectors.length > 0 ? filters.selectedSectors : undefined,
                countries: filters.selectedCountries.length > 0 ? filters.selectedCountries : undefined,
                page: filters.page,
                pageSize: filters.pageSize
            });
            setResults(response);
        } catch (error) {
            console.error("Error searching companies:", error);
        } finally {
            setIsLoading(false);
        }
    };

    const handlePageChange = (newPage: number) => {
        setFilters(prev => ({ ...prev, page: newPage }));
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };

    return (
        <div className="space-y-6 w-full max-w-5xl mx-auto p-4">
            {/* HEADER */}
            <div className="flex items-center gap-4">
                <Button variant="ghost" size="icon" onClick={onBack}><ArrowLeft size={20} /></Button>
                <h2 className="text-3xl font-bold italic tracking-tighter text-white">Find <span className="text-primary">Companies</span></h2>
            </div>

            {/* SEARCH BAR */}
            <div className="flex gap-2">
                <div className="relative flex-1">
                    <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                    <Input
                        placeholder="Search company by name..."
                        className="pl-10 h-12 bg-secondary/20 border-none text-lg text-white font-medium"
                        value={filters.name}
                        onChange={(e) => setFilters({ ...filters, name: e.target.value })}
                        onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
                    />
                </div>
                <Button
                    variant={showFilters ? "default" : "outline"}
                    className="h-12 px-6 gap-2 border-primary/20 hover:bg-primary/10 transition-colors"
                    onClick={() => setShowFilters(!showFilters)}
                >
                    <Filter size={18} /> {showFilters ? "Hide Filters" : "Filters"}
                </Button>
                <Button 
                    className="h-12 px-8 font-bold shadow-lg shadow-primary/20" 
                    onClick={() => {
                        setFilters(prev => ({ ...prev, page: 1 }));
                        handleSearch();
                    }} 
                    disabled={isLoading}
                >
                    {isLoading ? <Loader2 className="animate-spin" size={20} /> : "Search"}
                </Button>
            </div>

            {/* FILTERS PANEL */}
            {showFilters && (
                <Card className="border-none bg-secondary/10 animate-in slide-in-from-top-4 duration-300">
                    <CardContent className="p-6 grid grid-cols-1 md:grid-cols-3 gap-8">
                        
                        {/* Sectors */}
                        <div className="space-y-3">
                            <Label className="text-xs font-bold uppercase text-primary tracking-widest">Sectors</Label>
                            <div className="max-h-48 overflow-y-auto space-y-2 pr-2 custom-scrollbar">
                                {sectors.map(s => (
                                    <div key={s.id} className="flex items-center space-x-2 group">
                                        <Checkbox
                                            id={`s-${s.id}`}
                                            checked={filters.selectedSectors.includes(s.name)}
                                            onCheckedChange={(checked) => {
                                                setFilters(prev => ({
                                                    ...prev,
                                                    selectedSectors: checked
                                                        ? [...prev.selectedSectors, s.name]
                                                        : prev.selectedSectors.filter(name => name !== s.name)
                                                }));
                                            }}
                                        />
                                        <label htmlFor={`s-${s.id}`} className="text-sm text-white/70 cursor-pointer group-hover:text-primary transition-colors">{s.name}</label>
                                    </div>
                                ))}
                            </div>
                        </div>

                        {/* Countries (Now from API) */}
                        <div className="space-y-3">
                            <Label className="text-xs font-bold uppercase text-primary tracking-widest">Countries</Label>
                            <div className="relative mb-2">
                                <SearchIcon className="absolute left-2 top-1/2 -translate-y-1/2 text-muted-foreground" size={12} />
                                <input
                                    type="text"
                                    placeholder="Find country..."
                                    className="w-full bg-white/5 border border-white/10 rounded px-7 py-1 text-[10px] text-white outline-none focus:border-primary/50 transition-all"
                                    value={countrySearch}
                                    onChange={(e) => setCountrySearch(e.target.value)}
                                />
                            </div>
                            <div className="max-h-40 overflow-y-auto space-y-2 pr-2 custom-scrollbar">
                                {filteredCountriesList.map(country => (
                                    <div key={country} className="flex items-center space-x-2 group">
                                        <Checkbox
                                            id={`c-${country}`}
                                            checked={filters.selectedCountries.includes(country)}
                                            onCheckedChange={(checked) => {
                                                setFilters(prev => ({
                                                    ...prev,
                                                    selectedCountries: checked
                                                        ? [...prev.selectedCountries, country]
                                                        : prev.selectedCountries.filter(c => c !== country)
                                                }));
                                            }}
                                        />
                                        <label htmlFor={`c-${country}`} className="text-sm text-white/70 cursor-pointer group-hover:text-primary transition-colors">{country}</label>
                                    </div>
                                ))}
                            </div>
                        </div>

                        {/* Company Size */}
                        <div className="space-y-3">
                            <Label className="text-xs font-bold uppercase text-primary tracking-widest">Company Size</Label>
                            <select
                                className="w-full bg-secondary/40 border border-white/10 rounded-md p-2.5 text-sm text-white outline-none focus:border-primary transition-all appearance-none cursor-pointer"
                                value={filters.companySize}
                                onChange={(e) => setFilters({ ...filters, companySize: e.target.value })}
                            >
                                <option value="" className="bg-slate-900">Any Size</option>
                                {SIZE_OPTIONS.map(opt => (
                                    <option key={opt} value={opt} className="bg-slate-900">{opt} Employees</option>
                                ))}
                            </select>
                        </div>
                    </CardContent>
                </Card>
            )}

            {/* RESULTS GRID */}
            <div className="pt-10 border-t border-white/5">
                {isLoading && (
                    <div className="flex flex-col items-center justify-center py-20 gap-4">
                        <Loader2 className="animate-spin text-primary" size={40} />
                        <span className="text-xs text-muted-foreground animate-pulse uppercase tracking-widest">Fetching results...</span>
                    </div>
                )}

                {results?.items?.length > 0 && !isLoading && (
                    <div className="space-y-8">
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            {results.items.map((company: any) => (
                                <Card key={company.id} className="bg-secondary/5 border-none hover:bg-secondary/10 transition-all group relative overflow-hidden cursor-pointer">
                                    <div className="absolute top-0 left-0 w-1 h-full bg-primary opacity-0 group-hover:opacity-100 transition-all" />
                                    <CardContent className="p-6">
                                        <div className="flex gap-5">
                                            <div className="w-16 h-16 rounded-xl bg-gradient-to-br from-primary/20 to-secondary/20 flex items-center justify-center border border-white/5 group-hover:border-primary/30 transition-all">
                                                {company.logoUrl ? (
                                                    <img src={company.logoUrl} alt={company.companyName} className="w-full h-full object-cover rounded-xl" />
                                                ) : (
                                                    <Building2 className="text-primary/60 group-hover:text-primary transition-colors" size={32} />
                                                )}
                                            </div>
                                            <div className="flex-1 min-w-0">
                                                <h3 className="text-xl font-bold text-white group-hover:text-primary transition-colors truncate">{company.companyName}</h3>
                                                <div className="flex flex-wrap gap-2 mt-2">
                                                    {company.sectors?.slice(0, 3).map((s: string) => (
                                                        <span key={s} className="text-[9px] bg-primary/5 text-primary/80 px-2 py-0.5 rounded-full font-bold uppercase tracking-wider border border-primary/10">
                                                            {s}
                                                        </span>
                                                    ))}
                                                </div>
                                            </div>
                                        </div>
                                        <div className="mt-8 pt-4 border-t border-white/5 flex justify-between items-center">
                                            <div className="flex items-center gap-2 text-muted-foreground">
                                                <Globe size={14} className="text-primary/50" />
                                                <span className="text-xs font-medium text-white/60">{company.countryHeadquarters || "Global"}</span>
                                            </div>
                                            <Button 
                                                variant="ghost" 
                                                size="sm" 
                                                className="text-primary hover:text-primary hover:bg-primary/10 text-xs font-bold uppercase tracking-tighter"
                                                onClick={(e) => {
                                                    e.stopPropagation(); // Evita clicar no card e no botão ao mesmo tempo
                                                    router.push(`/company/${company.id}`);
                                                }}
                                            >
                                                Details
                                            </Button>
                                            <div className="bg-white/5 px-3 py-1 rounded-md text-[10px] font-bold text-primary uppercase tracking-tighter">
                                                {company.companySize} STAFF
                                            </div>
                                        </div>
                                    </CardContent>
                                </Card>
                            ))}
                        </div>

                        {/* PAGINATION */}
                        {results?.totalItems > filters.pageSize && (
                            <div className="flex items-center justify-center gap-4 pt-10 border-t border-white/5">
                                <Button
                                    variant="outline"
                                    size="sm"
                                    className="border-white/10 text-white hover:bg-primary/20"
                                    disabled={filters.page === 1 || isLoading}
                                    onClick={() => handlePageChange(filters.page - 1)}
                                >
                                    Previous
                                </Button>
                                
                                <div className="flex items-center gap-2">
                                    <span className="text-sm font-medium text-white/60">
                                        Page <span className="text-white">{filters.page}</span> of <span className="text-white">{Math.ceil(results.totalItems / filters.pageSize)}</span>
                                    </span>
                                </div>

                                <Button
                                    variant="outline"
                                    size="sm"
                                    className="border-white/10 text-white hover:bg-primary/20"
                                    disabled={filters.page >= Math.ceil(results.totalItems / filters.pageSize) || isLoading}
                                    onClick={() => handlePageChange(filters.page + 1)}
                                >
                                    Next
                                </Button>
                            </div>
                        )}
                    </div>
                )}

                {/* Empty State */}
                {results?.items?.length === 0 && !isLoading && (
                    <div className="text-center py-20 bg-secondary/5 rounded-2xl border border-dashed border-white/5">
                        <p className="text-white/40 font-medium">No matches found for your criteria.</p>
                        <Button variant="link" className="text-primary" onClick={() => setFilters({
                            name: '', companySize: '', selectedSectors: [], selectedCountries: [], page: 1, pageSize: 10
                        })}>Reset Filters</Button>
                    </div>
                )}
            </div>
        </div>
    );
}