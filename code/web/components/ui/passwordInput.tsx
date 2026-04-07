import { useState } from "react";
import { Input } from "./input";
import { Eye, EyeOff } from "lucide-react";

export function PasswordInput({
    label,
    value,
    onChange,
  }: {
    label: string;
    value: string;
    onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  }) {
    const [showPassword, setShowPassword] = useState(false);
  
    return (
      <div className="mb-4">
        <label className="ml-auto inline-block text-sm">
          {label}
        </label>
        <div className="relative">
          <Input
            type={showPassword ? 'text' : 'password'}
            value={value}
            onChange={onChange}
            className="pr-10"
          />
          <button
            type="button"
            onClick={() => setShowPassword((prev) => !prev)}
            className="absolute inset-y-0 right-0 flex items-center pr-2"
            aria-label={showPassword ? 'Hide password' : 'Show password'}
          >
            {showPassword ? <EyeOff className="h-5 w-5" /> : <Eye className="h-5 w-5" />}
          </button>
        </div>
      </div>
    );
  }