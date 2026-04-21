'use client';

import React, { useState, useEffect } from 'react';
import { Search, Filter, ArrowLeft, Loader2, Router } from 'lucide-react';
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { Checkbox } from "@/components/ui/checkbox";
import { useRouter } from 'next/navigation';

import creatorService from '@/services/api/CreatorService';
import sectorService, { Sector } from '@/services/api/SectorService';
import platformService, { SocialPlatform } from '@/services/api/PlatformService';

export function CreatorSearch({ onBack }: { onBack: () => void }) {
    const [showFilters, setShowFilters] = useState(false);
    const [platforms, setPlatforms] = useState<SocialPlatform[]>([]);
    const [sectors, setSectors] = useState<Sector[]>([]);
    const router = useRouter();


    const [isLoading, setIsLoading] = useState(false);
    const [results, setResults] = useState<any>(null);
    // Estado do Filtro seguindo o teu DTO

    const [filters, setFilters] = useState({
        platformUserName: '',
        platformId: undefined as number | undefined,
        followersCountMin: undefined as number | undefined,
        followersCountMax: undefined as number | undefined,
        priceMin: undefined as number | undefined,
        priceMax: undefined as number | undefined,
        selectedSectors: [] as string[],
        page: 1,
        pageSize: 10
    });

    // Carregar dados iniciais (Plataformas e Setores)
    useEffect(() => {
        const loadData = async () => {
            try {
                const [platData, sectData] = await Promise.all([
                    platformService.getAllPlatforms(),
                    sectorService.getAllSectors()
                ]);
                setPlatforms(platData);
                console.log("platData", platData)
                setSectors(sectData);
            } catch (err) {
                console.error("Error loading search metadata", err);
            }
        };
        loadData();
    }, []);


    const handlePageChange = (newPage: number) => {
        // Atualiza o filtro
        setFilters(prev => ({ ...prev, page: newPage }));

        // Pequeno truque: como o setFilters é assíncrono, 
        // usamos o useEffect abaixo para disparar a pesquisa
    };

    // Adiciona este useEffect para disparar a pesquisa sempre que a página mudar
    useEffect(() => {
        // Só dispara se já houver resultados (ou seja, se o utilizador já pesquisou uma vez)
        if (results) {
            handleSearch();
        }
    }, [filters.page]);

    const handleSectorToggle = (sectorName: string) => {
        setFilters(prev => ({
            ...prev,
            selectedSectors: prev.selectedSectors.includes(sectorName)
                ? prev.selectedSectors.filter(s => s !== sectorName)
                : [...prev.selectedSectors, sectorName]
        }));
    };

    const handleSearch = async () => {
        setIsLoading(true);
        try {
            console.log("Iniciando pesquisa com filtros:", filters);

            const response = await creatorService.searchCreators({
                platformId: filters.platformId,
                platformUserName: filters.platformUserName || undefined,
                followersCountMin: filters.followersCountMin,
                followersCountMax: filters.followersCountMax,
                priceMin: filters.priceMin,
                priceMax: filters.priceMax,
                sectors: filters.selectedSectors.length > 0 ? filters.selectedSectors : undefined,
                page: filters.page,
                pageSize: filters.pageSize
            });

            console.log("Resposta da API:", response);
            setResults(response);
        } catch (err) {
            console.error("Search error:", err);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="space-y-6 w-full max-w-5xl mx-auto">
            {/* HEADER & BACK */}
            <div className="flex items-center gap-4">
                <Button variant="ghost" size="icon" onClick={onBack}>
                    <ArrowLeft size={20} />
                </Button>
                <h2 className="text-3xl font-bold italic tracking-tighter">Find <span className="text-primary">Creators</span></h2>
            </div>

            {/* BARRA DE PESQUISA PRINCIPAL */}
            <div className="flex gap-2">
                <div className="relative flex-1">
                    <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                    <Input
                        placeholder="Search by username..."
                        className="pl-10 h-12 bg-secondary/20 border-none text-lg"
                        value={filters.platformUserName}
                        onChange={(e) => setFilters({ ...filters, platformUserName: e.target.value })}
                    />
                </div>
                <Button
                    variant={showFilters ? "default" : "outline"}
                    className="h-12 px-6 gap-2"
                    onClick={() => setShowFilters(!showFilters)}
                >
                    <Filter size={18} />
                    {showFilters ? "Hide Filters" : "Filters"}
                </Button>
                {/* Botão de pesquisa principal */}
                <Button
                    className="h-12 px-8 font-bold"
                    onClick={() => {
                        setFilters(prev => ({ ...prev, page: 1 })); // Reseta para página 1
                        handleSearch();
                    }}
                >
                    Search
                </Button>
            </div>

            {/* PAINEL DE FILTROS AVANÇADOS */}
            {showFilters && (
                <Card className="border-none bg-secondary/10 animate-in slide-in-from-top-4 duration-300">
                    <CardContent className="p-6 grid grid-cols-1 md:grid-cols-3 gap-8">

                        {/* PLATAFORMA */}
                        <div className="space-y-3">
                            <Label className="text-sm font-semibold uppercase tracking-wider text-muted-foreground">Platform</Label>
                            <div className="grid grid-cols-2 gap-2">
                                {platforms.map(p => (
                                    <Button
                                        key={p.id}
                                        variant={filters.platformId === p.id ? "default" : "outline"}
                                        size="sm"
                                        className="justify-start font-normal"
                                        onClick={() => setFilters({ ...filters, platformId: filters.platformId === p.id ? undefined : p.id })}
                                    >
                                        {p.namePlatform}
                                    </Button>
                                ))}
                            </div>
                        </div>

                        {/* RANGE DE SEGUIDORES E PREÇO */}
                        <div className="space-y-4">
                            <div className="space-y-2">
                                <Label className="text-sm font-semibold uppercase tracking-wider text-muted-foreground">Followers Range</Label>
                                <div className="flex items-center gap-2">
                                    <Input
                                        type="number" placeholder="Min"
                                        onChange={(e) => setFilters({ ...filters, followersCountMin: parseInt(e.target.value) || undefined })}
                                    />
                                    <span className="text-muted-foreground">-</span>
                                    <Input
                                        type="number" placeholder="Max"
                                        onChange={(e) => setFilters({ ...filters, followersCountMax: parseInt(e.target.value) || undefined })}
                                    />
                                </div>
                            </div>
                            <div className="space-y-2">
                                <Label className="text-sm font-semibold uppercase tracking-wider text-muted-foreground">Budget (Price Range)</Label>
                                <div className="flex items-center gap-2">
                                    <Input
                                        type="number" placeholder="Min €"
                                        onChange={(e) => setFilters({ ...filters, priceMin: parseFloat(e.target.value) || undefined })}
                                    />
                                    <span className="text-muted-foreground">-</span>
                                    <Input
                                        type="number" placeholder="Max €"
                                        onChange={(e) => setFilters({ ...filters, priceMax: parseFloat(e.target.value) || undefined })}
                                    />
                                </div>
                            </div>
                        </div>

                        {/* SETORES */}
                        <div className="space-y-3">
                            <Label className="text-sm font-semibold uppercase tracking-wider text-muted-foreground">Sectors</Label>
                            <div className="max-h-40 overflow-y-auto space-y-2 pr-2 custom-scrollbar">
                                {sectors.map(s => (
                                    <div key={s.id} className="flex items-center space-x-2">
                                        <Checkbox
                                            id={`s-${s.id}`}
                                            checked={filters.selectedSectors.includes(s.name)}
                                            onCheckedChange={() => handleSectorToggle(s.name)}
                                        />
                                        <label htmlFor={`s-${s.id}`} className="text-sm cursor-pointer">{s.name}</label>
                                    </div>
                                ))}
                            </div>
                        </div>

                    </CardContent>
                </Card>
            )}

            {/* ÁREA DE RESULTADOS DINÂMICA */}
            <div className="pt-10 border-t border-border/50">
                {!results && !isLoading && (
                    <div className="flex flex-col items-center justify-center py-12 text-muted-foreground">
                        <Search size={48} className="mb-4 opacity-20" />
                        <p>Enter criteria and search for creators.</p>
                    </div>
                )}

                {isLoading && (
                    <div className="flex justify-center py-20">
                        <Loader2 className="animate-spin text-primary" size={40} />
                    </div>
                )}

                {/* Nota: Usamos "items" em minúsculo conforme o teu JSON */}
                {results?.items?.length === 0 && !isLoading && (
                    <p className="text-center text-destructive py-10">No creators found with these filters.</p>
                )}

                {results?.items?.length > 0 && !isLoading && (
                    <div className="space-y-6">
                        <div className="flex justify-between items-center">
                            <p className="text-sm text-muted-foreground">
                                Found <strong>{results.totalItems}</strong> creators
                            </p>
                        </div>

                        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
                            {results.items.map((creator: any) => (
                                <Card key={creator.id} className="overflow-hidden border-none bg-secondary/5 hover:bg-secondary/10 transition-all duration-300 group">
                                    <CardContent className="p-0">
                                        {/* Header com Gradiente e Inicial */}
                                        <div className="h-24 bg-gradient-to-r from-primary/20 to-primary/5 flex items-end p-4">
                                            <div className="w-16 h-16 rounded-full bg-background border-4 border-secondary flex items-center justify-center text-xl font-bold shadow-lg">
                                                {creator.platformUserName?.charAt(0).toUpperCase()}
                                            </div>
                                        </div>

                                        <div className="p-4 pt-8">
                                            <div className="flex justify-between items-start mb-2">
                                                <div>
                                                    <h3 className="font-bold text-lg leading-tight group-hover:text-primary transition-colors truncate max-w-[150px]">
                                                        @{creator.platformUserName}
                                                    </h3>
                                                    {/* Setores - Pegamos no primeiro do array */}
                                                    <p className="text-[10px] text-muted-foreground mt-1">
                                                        {creator.sectors?.[0] || "General"}
                                                    </p>
                                                </div>
                                                <div className="text-right">
                                                    <p className="text-sm font-bold text-primary">
                                                        {new Intl.NumberFormat('pt-PT', { style: 'currency', currency: 'EUR' }).format(creator.priceMin || 0)}
                                                    </p>
                                                    <p className="text-[10px] text-muted-foreground uppercase tracking-tighter">Starting at</p>
                                                </div>
                                            </div>

                                            <div className="grid grid-cols-2 gap-4 mt-6 py-4 border-t border-border/50">
                                                <div>
                                                    <p className="text-[10px] uppercase text-muted-foreground font-semibold">Followers</p>
                                                    <p className="font-bold text-sm">
                                                        {Intl.NumberFormat('en-US', { notation: 'compact' }).format(creator.followersCount)}
                                                    </p>
                                                </div>
                                                <div>
                                                    <p className="text-[10px] uppercase text-muted-foreground font-semibold">Platform</p>
                                                    <p className="font-bold text-sm capitalize">
                                                        {/* Mapeamento simples de ID para Nome se não vier o nome da plataforma */}
                                                        {creator.platformId === 6 ? "TikTok" : creator.platformId === 2 ? "Instagram" : "YouTube"}
                                                    </p>
                                                </div>
                                            </div>

                                            <Button
                                                className="w-full mt-2 group-hover:bg-primary group-hover:text-primary-foreground"
                                                variant="outline"
                                                onClick={() => router.push(`/socialProfile/${creator.id}`)}
                                            >
                                                View Profile
                                            </Button>
                                        </div>
                                    </CardContent>
                                </Card>
                            ))}
                        </div>
                    </div>
                )}
            </div>

            {/* PAGINAÇÃO - Adicionámos o "?" após results */}
            {results?.totalItems > filters.pageSize && (
                <div className="flex items-center justify-center gap-4 pt-10">
                    <Button
                        variant="outline"
                        size="sm"
                        disabled={filters.page === 1 || isLoading}
                        onClick={() => handlePageChange(filters.page - 1)}
                    >
                        Previous
                    </Button>

                    <div className="flex items-center gap-2">
                        <span className="text-sm font-medium">
                            Page {filters.page} of {Math.ceil(results.totalItems / filters.pageSize)}
                        </span>
                    </div>

                    <Button
                        variant="outline"
                        size="sm"
                        disabled={filters.page >= Math.ceil(results.totalItems / filters.pageSize) || isLoading}
                        onClick={() => handlePageChange(filters.page + 1)}
                    >
                        Next
                    </Button>
                </div>
            )}
        </div>
    );
}