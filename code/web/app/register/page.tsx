import { Register } from '@/components/auth/Register';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';

export default function LoginPage() {
    return (
        <div className="flex min-h-svh w-full items-center justify-center p-6 md:p-10">
            <div className="w-full max-w-sm">
                <Card>
                    <CardHeader>
                        <CardTitle className="text-2xl">Register</CardTitle>
                        <CardDescription>
                            Enter your email below to register to your account
                        </CardDescription>
                    </CardHeader>
                    <CardContent>
                        <Register />
                    </CardContent>
                </Card>
            </div>
        </div>
    );
}