'use client'

import { useEffect, useState } from "react";
import { Card, CardContent } from "@/components/ui/card";
import { Building2, Loader2 } from "lucide-react";
import CompanyService from "@/services/api/CompanyService";
import { useRouter } from "next/navigation";

interface CompanyRecommendation {
  id: number;
  companyName: string;
  description?: string;
  countryHeadquarters?: string;
  sectors?: string[];
}

const PAGE_SIZE = 4;

export function CompanyRecommendationFeed() {
  const [recommendations, setRecommendations] = useState<CompanyRecommendation[]>([]);
  const [loading, setLoading] = useState(true);
  const [visibleCount, setVisibleCount] = useState(PAGE_SIZE);
  const router = useRouter();

  useEffect(() => {
    async function fetchRecommendations() {
      try {
        const data = await CompanyService.getRecommendedCompanies();
        setRecommendations(data || []);
      } catch (error) {
        console.error("Erro ao carregar recomendações de companies:", error);
        setRecommendations([]);
      } finally {
        setLoading(false);
      }
    }

    fetchRecommendations();
  }, []);

  const visibleRecommendations = recommendations.slice(0, visibleCount);
  const canSeeMore = recommendations.length > visibleCount;

  if (loading) {
    return (
      <div className="flex h-40 w-full items-center justify-center rounded-3xl border border-dashed border-border/50 bg-muted/30">
        <Loader2 className="animate-spin text-primary" size={28} />
      </div>
    );
  }

  if (!recommendations || recommendations.length === 0) {
    return (
      <div className="rounded-3xl border border-dashed border-border/50 bg-muted/10 p-8 text-center text-sm text-muted-foreground">
        Nenhuma recomendação de empresas disponível no momento.
      </div>
    );
  }

  return (
    <>
      <div className="grid gap-4 grid-cols-1 sm:grid-cols-2 xl:grid-cols-4">
        {visibleRecommendations.map((company) => (
          <Card
            key={company.id}
            onClick={() => router.push(`/company/${company.id}`)}
            className="cursor-pointer transition-all hover:-translate-y-1 hover:border-primary/70"
          >
            <CardContent className="space-y-3 p-4">
              <div className="flex items-center gap-3">
                <div className="flex h-10 w-10 items-center justify-center rounded-2xl bg-primary/10 text-primary">
                  <Building2 size={18} />
                </div>
                <div className="min-w-0">
                  <p className="font-semibold text-sm truncate">{company.companyName}</p>
                  <p className="text-[10px] text-muted-foreground truncate">{company.countryHeadquarters || "Localização não disponível"}</p>
                </div>
              </div>

              <p className="text-[12px] text-muted-foreground line-clamp-3">
                {company.description || "Sem descrição disponível."}
              </p>

              <div className="flex flex-wrap gap-1">
                {company.sectors?.slice(0, 2).map((sector) => (
                  <span key={sector} className="rounded-full bg-secondary/20 px-2 py-0.5 text-[10px] font-medium text-muted-foreground">
                    {sector}
                  </span>
                ))}
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {canSeeMore && (
        <div className="mt-5 flex justify-center">
          <button
            type="button"
            onClick={() => setVisibleCount((current) => Math.min(current + PAGE_SIZE, recommendations.length))}
            className="rounded-full border border-primary px-4 py-2 text-sm font-semibold text-primary transition hover:bg-primary/10"
          >
            Ver mais
          </button>
        </div>
      )}
    </>
  );
}
