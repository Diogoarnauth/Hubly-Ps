'use client';

import React, { useState, useRef } from 'react';
import { toastSuccess } from '../ToastImplementations';
import { useRegisterContext } from '@/providers/RegisterContext';
import authService from '@/services/api/UsersService';
import { useRouter } from 'next/navigation';
import { Input } from '../ui/input';
import { Label } from '@radix-ui/react-label';
import { Button } from '../ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../ui/card';
import { toastError } from '../ToastImplementations';

export function ConfirmEmail() {
  const [userEmail] = useRegisterContext();
  const [isResending, setIsResending] = useState(false);
  const router = useRouter();
  const [codeDigits, setCodeDigits] = useState(['', '', '', '', '', '']);
  const [isLoading, setIsLoading] = useState(false);
    
  const inputRefs = useRef<Array<HTMLInputElement | null>>([null, null, null, null, null, null]);

  const handleDigitChange = (index: number, value: string) => {
    if (value === '') {
      const newCodeDigits = [...codeDigits];
      newCodeDigits[index] = '';
      setCodeDigits(newCodeDigits);
      return;
    }
    
    const numericValue = value.replace(/[^0-9]/g, '');
    if (numericValue === '') return;
    
    const singleDigit = numericValue.charAt(0);
    const newCodeDigits = [...codeDigits];
    newCodeDigits[index] = singleDigit;
    setCodeDigits(newCodeDigits);
    
    if (singleDigit !== '' && index < 5) {
      inputRefs.current[index + 1]?.focus();
    }
  };

  const handleKeyDown = (index: number, e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Backspace') {
      if (codeDigits[index] !== '') {
        const newCodeDigits = [...codeDigits];
        newCodeDigits[index] = '';
        setCodeDigits(newCodeDigits);
      } else if (index > 0) {
        inputRefs.current[index - 1]?.focus();
      }
    }
    else if (e.key === 'ArrowLeft' && index > 0) {
      inputRefs.current[index - 1]?.focus();
    }
    else if (e.key === 'ArrowRight' && index < 5) {
      inputRefs.current[index + 1]?.focus();
    }
  };

  const handlePaste = (e: React.ClipboardEvent<HTMLInputElement>) => {
    e.preventDefault();
    const pastedData = e.clipboardData.getData('text');
    const numericOnly = pastedData.replace(/[^0-9]/g, '').slice(0, 6);
    
    const newCodeDigits = [...codeDigits];
    for (let i = 0; i < numericOnly.length; i++) {
      if (i < 6) newCodeDigits[i] = numericOnly[i];
    }
    setCodeDigits(newCodeDigits);
    const nextEmptyIndex = newCodeDigits.findIndex(digit => digit === '');
    inputRefs.current[nextEmptyIndex !== -1 ? nextEmptyIndex : 5]?.focus();
  };

  const confirmationCode = codeDigits.join('');
  const canSubmit = !isLoading && confirmationCode.length === 6;

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsLoading(true);
    
    if (!userEmail) {
      toastError('Erro de Sessão', 'O email do utilizador não foi encontrado. Recomece o registo.');
      setIsLoading(false);
      return;
    }
    
    const result = await authService.validateConfirmationCode(userEmail, confirmationCode);
    
    if (result) {
      sessionStorage.removeItem('hubly_register_email');
      router.push('/dashboard');
    } else {
      setIsLoading(false);
    }
  }

  async function handleResendEmail() {
    if (!userEmail) {
      toastError('Erro', 'Email não encontrado.');
      return;
    }

    setIsResending(true);
    try {
      const success = await authService.resendEmailConfirmation(userEmail);
      if (success) {
        toastSuccess('Email Sended', 'We have sent a new code to your inbox.');
      }
    } catch (error) {
      toastError('Error', 'Failed to resend confirmation email.');
    } finally {
      setIsResending(false);
    }
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-2xl">Confirm Email</CardTitle>
        <CardDescription>
          Enter your confirmation code below to confirm your email
        </CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="max-w-sm mx-auto space-y-4">
          <fieldset disabled={isLoading || isResending} className="space-y-4"></fieldset>
          <fieldset disabled={isLoading} className="space-y-4">
            <Label htmlFor="confirmationCode">Confirmation Code</Label>
            <div className="flex justify-between gap-2">
              {codeDigits.map((digit, index) => (
                <Input
                  key={index}
                  ref={(el) => {
                    inputRefs.current[index] = el;
                  }}
                  type="text"
                  inputMode="numeric"
                  pattern="[0-9]*"
                  maxLength={1}
                  className="w-12 h-12 text-center text-lg"
                  value={digit}
                  onChange={(e) => handleDigitChange(index, e.target.value)}
                  onKeyDown={(e) => handleKeyDown(index, e)}
                  onPaste={index === 0 ? handlePaste : undefined}
                  aria-label={`Digit ${index + 1}`}
                />
              ))}
            </div>
            <Button
              type="submit"
              disabled={!canSubmit}
              className="w-full mt-4"
            >
              {isLoading ? 'Verifying...' : 'Confirm Email'}
            </Button>
            <div className="text-center mt-6">
              <p className="text-sm text-muted-foreground mb-2">
                Didn&apos;t receive the code?
              </p>
              <Button
                type="button"
                variant="ghost" 
                size="sm"
                onClick={handleResendEmail}
                disabled={isResending || isLoading}
                className="text-blue-600 hover:text-blue-700 hover:bg-blue-50"
              >
                {isResending ? 'Sending...' : 'Resend Confirmation Email'}
              </Button>
            </div>
          </fieldset>
        </form>
      </CardContent>
    </Card>
  );
}