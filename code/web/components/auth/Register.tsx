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
  const [emailError, setEmailError] = useState('');
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(false);
  const [name, setUsername] = useState('');
  const [, setUserEmail] = useRegisterContext();


  const isEmailValid = (email: string) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  const isPasswordValid = (password: string) => password.length >= 6;
  const isPasswordMatch = (password: string, confirmPassword: string) => password === confirmPassword;

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsLoading(true);
    setError('');
    setEmailError('');

    try {
      const confirmationCode = await authService.register(email, password, name);

      if (confirmationCode) {
        setUserEmail(email);
        router.push('/register/confirmEmail');
      } else {
        setIsLoading(false);
        setError('Failed to register');
      }
    } catch (err: any) {
      setIsLoading(false);
      const backendResponse = err.response?.data;
      const errorTitle = backendResponse?.title;
      const errorDetail = backendResponse?.detail;

      if (errorTitle === "EmailAlreadyExists" || (errorDetail && errorDetail.includes("already registered"))) {
        setEmailError('There is already an account registered on that email');
      } else {
        setError('An unexpected error occurred');
      }
    }
  }

  const canSubmit =
    !isLoading &&
    isEmailValid(email) &&
    isPasswordValid(password) &&
    isPasswordMatch(password, confirmPassword);

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
              <Label htmlFor="email" className={emailError ? "text-destructive" : ""}>
                Email
              </Label>
              <Input
                id="email"
                type="email"
                value={email}
                onChange={(e) => {
                  setEmail(e.target.value);
                  if (emailError) setEmailError('');
                }}
                placeholder="example@gmail.com"
                required
                className={emailError ? "border-destructive focus-visible:ring-destructive" : ""}
              />
              {emailError && (
                <p className="text-xs font-medium text-destructive mt-1">
                  {emailError}
                </p>
              )}
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
            <div>
              <PasswordInput
                label="Password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
              <p className="text-[11px] text-muted-foreground mt-1.5 ml-1 ">
                The password must include 8 digits one UperCase, Numbers and Special caracter
              </p>
            </div>

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