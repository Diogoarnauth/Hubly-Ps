'use client';

import { DashboardProvider } from "@/providers/DashboardContext";

export default function DashboardLayout({
    children,
}: {
    children: React.ReactNode;
}) {
    return (
        <DashboardProvider>
            <div className="min-h-screen bg-background">
                {children}
            </div>
        </DashboardProvider>
    );
}