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
  const [name, setUsername] = useState('');
  const [, setUserEmail] = useRegisterContext();

  // 1. Removemos o estado 'redirect' (não é necessário se navegarmos no handleSubmit)

  const isEmailValid = (email: string) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  const isPasswordValid = (password: string) => password.length >= 6;
  const isPasswordMatch = (password: string, confirmPassword: string) => password === confirmPassword;

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsLoading(true);
    setError(''); // Limpamos erros anteriores ao tentar de novo

    try {
      const confirmationCode = await authService.register(email, password, name);
      
      if (confirmationCode) {
        // 2. Definimos o email no contexto (que agora grava no sessionStorage)
        setUserEmail(email);
        
        // 3. Navegamos IMEDIATAMENTE. O React não vai reclamar porque
        // isto acontece após um evento de clique, não durante o desenho da página.
        router.push('/register/confirmEmail');
      } else {
        setIsLoading(false);
        setError('Failed to register');
      }
    } catch (err) {
      setIsLoading(false);
      setError('An unexpected error occurred');
    }
  }

  const canSubmit =
    !isLoading &&
    isEmailValid(email) &&
    isPasswordValid(password) &&
    isPasswordMatch(password, confirmPassword);

  // 4. REMOVIDO: O bloco "if (redirect && !isLoading) { router.push(...) }"
  // Era aqui que o React "crashava".

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
              <p className="text-[11px] text-muted-foreground mt-1.5 ml-1 ">
                The password must include 8 digits, one UperCase, Numbers and Special caracter
              </p>

              <PasswordInput
                label="Confirm Password"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
              />

              <Button type="submit" disabled={!canSubmit} className="w-full">
                {isLoading ? 'Creating account...' : 'Register'}
              </Button>

              <p className="mt-4 text-center text-sm">
                Already have an account?{' '}
                <Link href="/" className="underline">
                  Login
                </Link>
              </p>

              {error && (
                  <Alert className="mt-4" variant="destructive">
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