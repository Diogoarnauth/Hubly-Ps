'use client';

import React from 'react';

export default function SocialProfileLayout({
    children,
}: {
    children: React.ReactNode;
}) {
    return (
        <div className="min-h-screen bg-background flex items-center justify-center">
            {/* O container principal mantém o estilo do Onboarding e Login */}
            <div className="w-full max-w-2xl p-6 md:p-10">
                {children}
            </div>
        </div>
    );
}