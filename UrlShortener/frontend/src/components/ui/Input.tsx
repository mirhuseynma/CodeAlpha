import React, { type InputHTMLAttributes } from 'react';
import { cn } from './Button'; // Reusing cn utility

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
}

const Input = React.forwardRef<HTMLInputElement, InputProps>(
  ({ className, label, error, ...props }, ref) => {
    return (
      <div className="w-full flex flex-col gap-1.5">
        {label && (
          <label className="text-sm font-medium text-slate-300">
            {label}
          </label>
        )}
        <input
          ref={ref}
          className={cn(
            "glass-input w-full",
            error && "border-red-500/50 focus:ring-red-500",
            className
          )}
          {...props}
        />
        {error && (
          <span className="text-sm text-red-400 mt-1">{error}</span>
        )}
      </div>
    );
  }
);

Input.displayName = 'Input';
export default Input;
