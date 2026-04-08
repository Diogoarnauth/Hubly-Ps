'use client';

import React, { useState } from 'react';
import creatorService from '@/services/api/CreatorService'; // Importamos o service
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import { useRouter } from 'next/navigation';

export function CreatorForm({ onBack }: { onBack: () => void }) {
  const [artisticName, setArtisticName] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const router = useRouter();

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    setIsLoading(true);
    setError('');

    try {
      const response = await creatorService.registerCreator(artisticName);
      
      if (response) {
        router.push('/dashboard'); 
      } else {
        setError('Failed to register as creator.');
      }
    } catch (err) {
      setError('An unexpected error occurred.');
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <Card className="border-border bg-card/50">
      <CardHeader>
        <CardTitle className="text-2xl">Creator Profile</CardTitle>
        <CardDescription>Tell us your artistic name to get started.</CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="artisticName">Artistic Name</Label>
            <Input
              id="artisticName"
              value={artisticName}
              onChange={(e) => setArtisticName(e.target.value)}
              placeholder="Artistic Name"
              required
              disabled={isLoading}
            />
          </div>

          <Button type="submit" className="w-full" disabled={isLoading || !artisticName}>
            {isLoading ? 'Registering...' : 'Complete Registration'}
          </Button>

          <Button type="button" variant="ghost" className="w-full" onClick={onBack} disabled={isLoading}>
            ← Back
          </Button>

          {error && (
            <Alert variant="destructive">
              <AlertTitle>Error</AlertTitle>
              <AlertDescription>{error}</AlertDescription>
            </Alert>
          )}
        </form>
      </CardContent>
    </Card>
  );
}