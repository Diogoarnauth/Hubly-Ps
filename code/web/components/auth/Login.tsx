'use client';

import React, { useState } from 'react';
import authService from '@/services/api/UsersService';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import { PasswordInput } from '@/components/ui/passwordInput';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../ui/card';
import { toastSuccess } from '../ToastImplementations';

export function LoginForm() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const router = useRouter();

    async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();
        setIsLoading(true);
        const result = await authService.login(email, password);
        if (result) {
            toastSuccess('Login successful', 'You are now logged in');
            router.push('/dashboard');
        }else{
            setPassword('');
            setIsLoading(false);
        }
    }
/*
    const isEmailValid = (email: string): boolean =>
        /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);

    const isPasswordValid = (password: string): boolean =>
        password.length >= 6;
    */

    return (
        <Card>
            <CardHeader>
            <CardTitle className="text-2xl">Login</CardTitle>
            <CardDescription>
                Enter your email below to login to your account
            </CardDescription>
            </CardHeader>
            <CardContent>
                <form onSubmit={handleSubmit} className="max-w-sm mx-auto space-y-4">
                    <fieldset disabled={isLoading} className="space-y-4">
                        <div>
                            <Label htmlFor="email">Email</Label>
                            <Input
                                id="email"
                                type="email"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                placeholder="example@email.com"
                                required
                            />
                        </div>

                        <PasswordInput
                            label="Password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                        />

                        <Button
                            type="submit"
                            disabled={isLoading }//|| !isEmailValid(email) || !isPasswordValid(password)
                            className="w-full"
                        >
                            {isLoading ? 'Logging in...' : 'Login'}
                        </Button>

                        <p className="mt-4 text-center text-sm">
                            Do not have an account?{' '}
                            <Link href="/register" className="underline">
                                Register
                            </Link>
                        </p>
                    </fieldset>
                </form>
            </CardContent>
        </Card>
    );
}
