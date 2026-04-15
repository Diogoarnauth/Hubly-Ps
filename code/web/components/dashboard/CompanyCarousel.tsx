'use client';

import React, { useEffect, useState } from 'react';
import { Card, CardContent } from "@/components/ui/card";
import { Carousel, CarouselContent, CarouselItem, CarouselNext, CarouselPrevious } from "@/components/ui/carousel";
import { Building2, Loader2, MapPin } from "lucide-react";
import CompanyService, { TrendingCompany } from "@/services/api/CompanyService";

export function CompanyCarousel() {
    const [companies, setCompanies] = useState<TrendingCompany[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        async function fetchTrendingCompanies() {
            try {
                const data = await CompanyService.getTrendingCompanies(15);
                console.log("Trending Companies Data:", data);
                setCompanies(data || []);
            } catch (error) {
                console.error("Erro ao carregar trending companies:", error);
                setCompanies([]);
            } finally {
                setLoading(false);
            }
        }

        fetchTrendingCompanies();
    }, []);

    if (loading) {
        return (
            <div className="flex h-64 w-full items-center justify-center">
                <Loader2 className="animate-spin text-primary" size={32} />
            </div>
        );
    }

    if (companies.length === 0) {
        return (
            <div className="text-center p-10 border border-dashed rounded-lg text-muted-foreground">
                Nenhuma empresa em destaque.
            </div>
        );
    }

    return (
        <div className="relative w-full px-12">
            <Carousel opts={{ align: "start", loop: true }} className="w-full">
                <CarouselContent className="-ml-4">
                    {companies.map((company) => (
                        <CarouselItem key={company.user_id} className="pl-4 basis-full sm:basis-1/2 md:basis-1/3 lg:basis-1/5">
                            <Card className="overflow-hidden hover:border-primary/50 transition-all cursor-pointer bg-secondary/10 h-full flex flex-col">
                                {/* Área Visual */}
                                <div className="aspect-video bg-muted flex items-center justify-center border-b shrink-0">
                                    <Building2 className="text-muted-foreground/40" size={40} />
                                </div>

                                <CardContent className="p-4 flex flex-col flex-grow">
                                    {/* Nome da Empresa */}
                                    <h3 className="font-bold text-sm truncate uppercase tracking-tight" title={company.company_name}>
                                        {company.company_name}
                                    </h3>

                                    {/* Localização */}
                                    <div className="flex items-center gap-1 mt-1 text-muted-foreground">
                                        <MapPin size={10} />
                                        <span className="text-[10px] font-medium">
                                            {company.country_headquarters}
                                        </span>
                                    </div>

                                    {/* Descrição */}
                                    <p className="text-[10px] text-muted-foreground line-clamp-2 mt-2 min-h-[2.5em]">
                                        {company.description || "No description available."}
                                    </p>

                                    {/* Sectors - Badge Style */}
                                    <div className="mt-3 flex flex-wrap gap-1">
                                        {company.sectors?.slice(0, 2).map((sector, idx) => (
                                            <span
                                                key={idx}
                                                className="bg-primary/10 text-primary text-[8px] px-1.5 py-0.5 rounded font-semibold uppercase"
                                            >
                                                {sector}
                                            </span>
                                        ))}
                                        {company.sectors?.length > 2 && (
                                            <span className="text-[8px] text-muted-foreground self-center">
                                                +{company.sectors.length - 2}
                                            </span>
                                        )}
                                    </div>
                                </CardContent>
                            </Card>
                        </CarouselItem>
                    ))}
                </CarouselContent>
                <CarouselPrevious className="hidden md:flex" />
                <CarouselNext className="hidden md:flex" />
            </Carousel>
        </div>
    );
}