'use client';
import { Button } from "@/components/ui/button";

export function CompanyForm({ onBack }: { onBack: () => void }) {
  return (
    <div className="text-center">
      <h2 className="text-2xl font-bold">Company Registration</h2>
      <p className="mb-4">Soon: Multi-step process</p>
      <Button variant="link" onClick={onBack}>← Back to selection</Button>
    </div>
  );
}