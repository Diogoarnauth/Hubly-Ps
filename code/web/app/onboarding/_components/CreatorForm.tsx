'use client';
import { Button } from "@/components/ui/button";

export function CreatorForm({ onBack }: { onBack: () => void }) {
  return (
    <div className="text-center">
      <h2 className="text-2xl font-bold">Creator Registration</h2>
      <p className="mb-4">Soon: Input for Artistic Name</p>
      <Button variant="link" onClick={onBack}>← Back to selection</Button>
    </div>
  );
}