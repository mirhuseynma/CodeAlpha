import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { Link, useNavigate } from 'react-router-dom';
import { loginSchema, type LoginFormData } from './schemas';
import Input from '../../components/ui/Input';
import Button from '../../components/ui/Button';
import { useAuth } from './AuthContext';
import api, { setTokens } from '../../services/api';

const LoginForm = () => {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [apiError, setApiError] = useState<string | null>(null);
  
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFormData) => {
    try {
      setApiError(null);
      const response = await api.post('/auth/login', data);
      
      const { accessToken, refreshToken } = response.data;
      const rememberMe = data.rememberMe ?? true;
      setTokens(accessToken, refreshToken, rememberMe);
      // Store email for refresh operations if needed
      const storage = rememberMe ? localStorage : sessionStorage;
      storage.setItem('userEmail', data.email);
      
      login();
      navigate('/dashboard');
    } catch (error: any) {
      if (error.response?.data?.message) {
        setApiError(error.response.data.message);
      } else {
        setApiError('Invalid credentials or server error.');
      }
    }
  };

  return (
    <div className="w-full max-w-md p-8 glass-panel rounded-2xl mx-auto">
      <div className="text-center mb-8">
        <h2 className="text-3xl font-bold text-white mb-2">Welcome Back</h2>
        <p className="text-slate-400">Sign in to your LinkForge account</p>
      </div>

      {apiError && (
        <div className="bg-red-500/10 border border-red-500/50 text-red-400 px-4 py-3 rounded-lg mb-6">
          {apiError}
        </div>
      )}

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
        <Input
          label="Email Address"
          type="email"
          placeholder="you@example.com"
          {...register('email')}
          error={errors.email?.message}
        />
        
        <Input
          label="Password"
          type="password"
          placeholder="••••••••"
          {...register('password')}
          error={errors.password?.message}
        />

        <div className="flex items-center justify-between text-sm">
          <label className="flex items-center gap-2 text-slate-300 cursor-pointer">
            <input type="checkbox" {...register('rememberMe')} className="rounded border-slate-700 bg-slate-800 text-indigo-500 focus:ring-indigo-500" />
            <span>Remember me</span>
          </label>
          <a href="#" className="text-indigo-400 hover:text-indigo-300 transition-colors">
            Forgot password?
          </a>
        </div>

        <Button type="submit" className="w-full mt-2" isLoading={isSubmitting}>
          Sign In
        </Button>
      </form>

      <p className="mt-6 text-center text-slate-400 text-sm">
        Don't have an account?{' '}
        <Link to="/register" className="text-indigo-400 hover:text-indigo-300 font-medium transition-colors">
          Create one now
        </Link>
      </p>
    </div>
  );
};

export default LoginForm;
