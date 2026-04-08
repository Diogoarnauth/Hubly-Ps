'use client';
import React, { useState } from 'react';
import { useRouter } from 'next/navigation';
import authService from '@/services/api/UsersService';
import { useRegisterContext } from '@/providers/RegisterContext';
import Link from 'next/link';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { PasswordInput } from '@/components/ui/passwordInput';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../ui/card';
import { Label } from '../ui/label';
import { Alert, AlertDescription, AlertTitle } from '../ui/alert';


export function Register() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(false);
  const [redirect, setRedirect] = useState(false);
  const [name, setUsername] = useState('');
  const [, setUserEmail] = useRegisterContext();

  const isEmailValid = (email: string) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  const isPasswordValid = (password: string) => password.length >= 6;
  const isPasswordMatch = (password: string, confirmPassword: string) => password === confirmPassword;

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsLoading(true);
    const confirmationCode = await authService.register(email, password, name);
    if (confirmationCode) {
      setIsLoading(false);
      setUserEmail(email);
      setRedirect(true);
    } else {
      setError('Failed to register');
    }
  }

  const canSubmit =
    !isLoading &&
    isEmailValid(email) &&
    isPasswordValid(password) &&
    isPasswordMatch(password, confirmPassword);

  if (redirect && !isLoading) {
    router.push('/register/confirmEmail');
  }

  return (
    <Card>
        <CardHeader>
        <CardTitle className="text-2xl">Register</CardTitle>
        <CardDescription>
            Enter your email below to register to your account
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
              <div>
                <Label htmlFor="name">Name</Label>
                <Input
                  id="name"
                  type="text"
                  value={name}
                  onChange={(e) => setUsername(e.target.value)}
                  placeholder="Name"
                  required
                />
              </div>
              <PasswordInput
                label="Password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />

              <PasswordInput
                label="Confirm Password"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
              />

              <Button type="submit" disabled={!canSubmit} className="w-full">
                Register
              </Button>

              <p className="mt-4 text-center text-sm">
                Already have an account?{' '}
                <Link href="/" className="underline">
                  Login
                </Link>
              </p>

              {isLoading && <div>Carregando...</div>}
              {error && (
                  <Alert className="mt-4">
                      <AlertTitle>Error</AlertTitle>
                      <AlertDescription>{error}</AlertDescription>
                  </Alert>
              )}
            </fieldset>
          </form>
      </CardContent>
    </Card>
  );
}
