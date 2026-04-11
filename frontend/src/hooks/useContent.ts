import { useCallback } from 'react';
import { useContentStore } from '../store/contentStore';
import { contentService } from '../services/contentService';
import type { GeneratePostRequest } from '../types/content';

export function useContent() {
  const { setVariations, setGenerating } = useContentStore();

  const generatePost = useCallback(
    async (data: GeneratePostRequest) => {
      setGenerating(true);
      try {
        const response = await contentService.generatePost(data);
        const variations = response.variations.map((v, i) => ({
          ...v,
          id: v.id ?? `variation-${i}-${Date.now()}`,
          hashtags: v.hashtags.map((h) => h.replace(/^#/, '')),
        }));
        setVariations(variations);
        return variations;
      } catch (err: unknown) {
        const message =
          (err as { response?: { data?: { message?: string } } })?.response?.data?.message ||
          'Failed to generate content.';
        throw new Error(message);
      } finally {
        setGenerating(false);
      }
    },
    [setVariations, setGenerating]
  );

  return { generatePost };
}
