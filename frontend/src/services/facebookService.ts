import api from './api';
import type { FacebookPageDto, InsightsDto } from '../types/facebook';

export const facebookService = {
  initiateConnect: () =>
    api.post<{ redirectUrl: string }>('/facebook/connect').then((r) => r.data),

  callback: (code: string, state?: string) =>
    api.get('/facebook/callback', { params: { code, state } }).then((r) => r.data),

  getPages: () =>
    api.get<FacebookPageDto[]>('/facebook/pages').then((r) => r.data),

  disconnectPage: (pageId: string) =>
    api.delete(`/facebook/pages/${pageId}/disconnect`).then((r) => r.data),

  getInsights: (pageId: string) =>
    api.get<InsightsDto>(`/facebook/pages/${pageId}/insights`).then((r) => r.data),
};
