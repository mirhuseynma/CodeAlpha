import { z } from 'zod';

export const createLinkSchema = z.object({
  originalUrl: z.string().url('Please enter a valid URL'),
  customAlias: z.string().regex(/^[a-zA-Z0-9-_]*$/, 'Alias can only contain letters, numbers, hyphens, and underscores').optional().or(z.literal('')),
  expiresAt: z.string().optional().or(z.literal('')),
});

export type CreateLinkFormData = z.infer<typeof createLinkSchema>;
