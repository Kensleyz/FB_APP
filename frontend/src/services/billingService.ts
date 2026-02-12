import api from './api';
import type {
  PlanDto,
  SubscriptionDto,
  UsageDto,
  TransactionDto,
  SubscribeRequest,
  UpgradeRequest,
} from '../types/billing';

export const billingService = {
  getPlans: () =>
    api.get<PlanDto[]>('/billing/plans').then((r) => r.data),

  subscribe: (data: SubscribeRequest) =>
    api.post<{ paymentUrl: string }>('/billing/subscribe', data).then((r) => r.data),

  getSubscription: () =>
    api.get<SubscriptionDto>('/billing/subscription').then((r) => r.data),

  cancelSubscription: () =>
    api.post('/billing/subscription/cancel').then((r) => r.data),

  upgrade: (data: UpgradeRequest) =>
    api.post('/billing/subscription/upgrade', data).then((r) => r.data),

  downgrade: (data: UpgradeRequest) =>
    api.post('/billing/subscription/downgrade', data).then((r) => r.data),

  getUsage: () =>
    api.get<UsageDto>('/billing/usage').then((r) => r.data),

  getTransactions: () =>
    api.get<TransactionDto[]>('/billing/transactions').then((r) => r.data),
};
