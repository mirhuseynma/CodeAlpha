import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { createLinkSchema, type CreateLinkFormData } from './schemas';
import Input from '../../components/ui/Input';
import Button from '../../components/ui/Button';
import api from '../../services/api';
import LinkResultPanel from './LinkResultPanel';

export interface ShortLinkResponse {
  id: string;
  shortUrl: string;
  shortCode: string;
  originalUrl: string;
  customAlias?: string;
  createdAt: string;
  totalClicks: number;
  isActive: boolean;
  expiresAt?: string;
}

const CreateLinkForm = () => {
  const queryClient = useQueryClient();
  const [resultModalOpen, setResultModalOpen] = useState(false);
  const [createdLink, setCreatedLink] = useState<ShortLinkResponse | null>(null);
  
  const { register, handleSubmit, reset, formState: { errors } } = useForm<CreateLinkFormData>({
    resolver: zodResolver(createLinkSchema),
  });

  const mutation = useMutation({
    mutationFn: async (data: CreateLinkFormData) => {
      const response = await api.post<ShortLinkResponse>('/links', data);
      return response.data;
    },
    onSuccess: (data) => {
      setCreatedLink(data);
      setResultModalOpen(true);
      reset();
      // Invalidate the links list query so it refreshes (to be implemented later)
      queryClient.invalidateQueries({ queryKey: ['links'] });
    },
  });

  const onSubmit = (data: CreateLinkFormData) => {
    let formattedExpiresAt = undefined;
    if (data.expiresAt) {
      const date = new Date(data.expiresAt);
      const tzo = -date.getTimezoneOffset();
      const dif = tzo >= 0 ? '+' : '-';
      const pad = (num: number) => (num < 10 ? '0' : '') + num;
      formattedExpiresAt = date.getFullYear() +
        '-' + pad(date.getMonth() + 1) +
        '-' + pad(date.getDate()) +
        'T' + pad(date.getHours()) +
        ':' + pad(date.getMinutes()) +
        ':' + pad(date.getSeconds()) +
        dif + pad(Math.floor(Math.abs(tzo) / 60)) +
        ':' + pad(Math.abs(tzo) % 60);
    }

    const payload = {
      ...data,
      expiresAt: formattedExpiresAt,
    };
    mutation.mutate(payload as CreateLinkFormData);
  };

  return (
    <>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        {mutation.isError && (
          <div className="bg-red-500/10 border border-red-500/50 text-red-400 px-4 py-3 rounded-lg">
            {(mutation.error as any).response?.data?.message || 'Failed to create link. Try again.'}
            {(mutation.error as any).response?.data?.errors && (
              <ul className="list-disc ml-5 mt-2 text-sm text-red-300">
                {Object.entries((mutation.error as any).response.data.errors).map(([key, val]: any) => (
                  <li key={key}>{val.join(', ')}</li>
                ))}
              </ul>
            )}
          </div>
        )}
        
        <Input
          placeholder="Paste your long URL here... e.g. https://example.com/very/long/path"
          {...register('originalUrl')}
          error={errors.originalUrl?.message}
          className="py-4 text-lg"
        />
        
        <div className="flex gap-4 items-start">
          <div className="flex-1">
            <Input
              placeholder="Custom alias (optional)"
              {...register('customAlias')}
              error={errors.customAlias?.message}
            />
          </div>
          <div className="flex-1">
            <Input
              type="datetime-local"
              placeholder="Expiration Date (optional)"
              {...register('expiresAt')}
              error={errors.expiresAt?.message}
            />
          </div>
          <Button 
            type="submit" 
            isLoading={mutation.isPending}
            className="px-8 py-3.5"
          >
            Shorten
          </Button>
        </div>
      </form>

      {createdLink && (
        <LinkResultPanel 
          isOpen={resultModalOpen}
          onClose={() => setResultModalOpen(false)}
          link={createdLink}
        />
      )}
    </>
  );
};

export default CreateLinkForm;
