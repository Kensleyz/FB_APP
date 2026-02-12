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
        setVariations(response.variations);
        return response.variations;
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
