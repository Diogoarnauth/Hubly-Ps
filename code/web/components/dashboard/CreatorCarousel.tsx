'use client'

import { useEffect, useState } from "react"
import { Carousel, CarouselContent, CarouselItem, CarouselNext, CarouselPrevious } from "@/components/ui/carousel"
import { Card, CardContent } from "@/components/ui/card"
import { User, Loader2 } from "lucide-react"
import CreatorService, { TrendingCreator } from "@/services/api/CreatorService" 

export function CreatorCarousel() {
  // Inicializamos sempre como array vazio para evitar o erro do .map()
  const [creators, setCreators] = useState<TrendingCreator[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    async function fetchTrending() {
      try {
        const data = await CreatorService.getTrendingCreators(15)
        console.log("DADOS RECEBIDOS DA API:", data)
        
        // Se a API retornar null por algum erro de rede/auth, mantemos array vazio
        setCreators(data || [])
      } catch (error) {
        console.error("Erro ao carregar trending creators:", error)
        setCreators([])
      } finally {
        setLoading(false)
      }
    }

    fetchTrending()
  }, [])

  if (loading) {
    return (
      <div className="flex h-64 w-full items-center justify-center">
        <Loader2 className="animate-spin text-primary" size={32} />
      </div>
    )
  }

  // Se após o loading não houver nada, mostramos um estado vazio amigável
  if (!creators || creators.length === 0) {
    return (
      <div className="text-center p-10 border border-dashed rounded-lg text-muted-foreground">
        Nenhum criador em destaque no momento.
      </div>
    )
  }

  return (
    <div className="relative w-full px-12"> 
      <Carousel opts={{ align: "start", loop: true }}>
        <CarouselContent>
          {creators.map((c, index) => (
            <CarouselItem key={c.socialProfile_id || index} className="basis-full sm:basis-1/2 md:basis-1/3 lg:basis-1/5">
              <Card className="hover:border-primary transition-all group cursor-pointer h-full">
                <CardContent className="p-0">
                  <div className="flex aspect-[3/4] items-center justify-center bg-muted group-hover:bg-primary/5 transition-colors">
                    <User size={48} className="text-muted-foreground/30 group-hover:text-primary/30" />
                  </div>
                  
                  <div className="p-4 border-t">
                    {/* Nota: Se no console vires as letras minúsculas, 
                        muda c.PlatformUserName para c.platformUserName */}
                    <p className="font-bold truncate">
                      {c.PlatformUserName || (c as any).platformUserName || "Sem nome"}
                    </p>
                    <p className="text-xs text-muted-foreground italic">
                      {c.PlatformName || (c as any).platformName || "Plataforma"}
                    </p>
                    <p className="text-[10px] text-muted-foreground line-clamp-1 mt-1">
                      {c.Description || (c as any).description || ""}
                    </p>
                  </div>
                </CardContent>
              </Card>
            </CarouselItem>
          ))}
        </CarouselContent>
        <CarouselPrevious />
        <CarouselNext />
      </Carousel>
    </div>
  )
}