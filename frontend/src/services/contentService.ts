import api from './api';
import type {
  GeneratePostRequest,
  GeneratePostResponse,
  GenerateImageRequest,
  GenerateImageResponse,
} from '../types/content';

export const contentService = {
  generatePost: (data: GeneratePostRequest) =>
    api.post<{ isSuccess: boolean; data: GeneratePostResponse; errors: string[] }>('/content/generate', data)
      .then((r) => r.data.data),

  generateImage: (data: GenerateImageRequest) =>
    api.post<GenerateImageResponse>('/content/images/generate', data).then((r) => r.data),

  getTemplates: () =>
    api.get('/content/templates').then((r) => r.data),
};
