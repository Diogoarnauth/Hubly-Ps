import { OnboardingProvider } from "@/providers/OnboardingContext";

export default function OnboardingLayout({
    children,
}: Readonly<{
    children: React.ReactNode;
}>) {
    return (
        <OnboardingProvider>
            {children}
        </OnboardingProvider>
    );
}