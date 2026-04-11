import api from './api';
import type { DashboardOverview } from '../types/dashboard';
import type {
  ScheduleDto,
  CreateScheduleRequest,
  UpdateScheduleRequest,
  CalendarResponse,
} from '../types/dashboard';

export const dashboardService = {
  getOverview: () =>
    api.get('/dashboard/overview').then((r) => {
      const d = r.data.data;
      return {
        connectedPages: d.usage.pagesConnected,
        totalPagesAllowed: d.usage.pagesLimit,
        postsThisMonth: d.usage.postsGenerated,
        postsLimit: d.usage.postsLimit,
        imagesThisMonth: d.usage.imagesCreated,
        imagesLimit: d.usage.imagesLimit,
        upcomingPosts: d.scheduledPosts,
        engagementRate: 0,
        recentPosts: [],
      } as DashboardOverview;
    }),

  getAnalytics: (pageId: string, period = '30d') =>
    api.get(`/dashboard/analytics`, { params: { pageId, period } }).then((r) => r.data),
};

export const scheduleService = {
  create: (data: CreateScheduleRequest) =>
    api.post('/schedule', data).then((r) => r.data.data as ScheduleDto),

  list: () =>
    api.get('/schedule').then((r) => r.data.data as ScheduleDto[]),

  get: (id: string) =>
    api.get(`/schedule/${id}`).then((r) => r.data.data as ScheduleDto),

  update: (id: string, data: UpdateScheduleRequest) =>
    api.put(`/schedule/${id}`, data).then((r) => r.data.data as ScheduleDto),

  delete: (id: string) =>
    api.delete(`/schedule/${id}`).then((r) => r.data),

  publishNow: (id: string) =>
    api.post(`/schedule/${id}/publish-now`).then((r) => r.data),

  getCalendar: (month: string) =>
    api.get(`/schedule/calendar`, { params: { month } }).then((r) => r.data.data as CalendarResponse),
};
