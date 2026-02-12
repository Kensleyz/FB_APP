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
    api.get<DashboardOverview>('/dashboard/overview').then((r) => r.data),

  getAnalytics: (pageId: string, period = '30d') =>
    api.get(`/dashboard/analytics`, { params: { pageId, period } }).then((r) => r.data),
};

export const scheduleService = {
  create: (data: CreateScheduleRequest) =>
    api.post<ScheduleDto>('/schedule', data).then((r) => r.data),

  list: () =>
    api.get<ScheduleDto[]>('/schedule').then((r) => r.data),

  get: (id: string) =>
    api.get<ScheduleDto>(`/schedule/${id}`).then((r) => r.data),

  update: (id: string, data: UpdateScheduleRequest) =>
    api.put<ScheduleDto>(`/schedule/${id}`, data).then((r) => r.data),

  delete: (id: string) =>
    api.delete(`/schedule/${id}`).then((r) => r.data),

  publishNow: (id: string) =>
    api.post(`/schedule/${id}/publish-now`).then((r) => r.data),

  getCalendar: (month: string) =>
    api.get<CalendarResponse>(`/schedule/calendar`, { params: { month } }).then((r) => r.data),
};
