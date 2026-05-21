'use client'

import { useEffect, useState } from "react"
import { Carousel, CarouselContent, CarouselItem, CarouselNext, CarouselPrevious } from "@/components/ui/carousel"
import { Card, CardContent } from "@/components/ui/card"
import { User, Loader2 } from "lucide-react"
import CreatorService, { TrendingCreator } from "@/services/api/CreatorService" 
import { useRouter } from "next/navigation"

export function CreatorCarousel() {
  const [creators, setCreators] = useState<TrendingCreator[]>([])
  const [loading, setLoading] = useState(true)
  const router = useRouter()

  useEffect(() => {
    async function fetchTrending() {
      try {
        const data = await CreatorService.getTrendingCreators(15)
        console.log("DADOS RECEBIDOS DA API:", data)
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

  if (!creators || creators.length === 0) {
    return (
      <div className="text-center p-10 border border-dashed rounded-lg text-muted-foreground">
        No trending creators at the moment.
      </div>
    )
  }

  return (
    <div className="relative w-full px-12"> 
      <Carousel opts={{ align: "start", loop: true }}>
        <CarouselContent>
          {creators.map((c, index) => (
            <CarouselItem key={c.socialProfile_id || index} className="basis-full sm:basis-1/2 md:basis-1/3 lg:basis-1/5">
              <Card 
                onClick={() => router.push(`/socialProfile/${c.socialProfile_id}`)}
                className="hover:border-primary transition-all group cursor-pointer h-full active:scale-[0.98]"
              >
                <CardContent className="p-0">
                  <div className="flex aspect-[3/4] items-center justify-center bg-muted group-hover:bg-primary/5 transition-colors">
                    <User size={48} className="text-muted-foreground/30 group-hover:text-primary/30" />
                  </div>
                  
                  <div className="p-4 border-t">
                    <p className="font-bold truncate">
                      {c.PlatformUserName || (c as any).platformUserName || "No name"}
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