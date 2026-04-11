import api from './api';
import type {
  PlanDto,
  SubscriptionDto,
  UsageDto,
  TransactionDto,
  SubscribeRequest,
  UpgradeRequest,
} from '../types/billing';

interface ApiResult<T> {
  isSuccess: boolean;
  data: T;
  errors: string[];
}

export const billingService = {
  getPlans: () =>
    api.get<ApiResult<PlanDto[]>>('/billing/plans').then((r) => r.data.data),

  subscribe: (data: SubscribeRequest) =>
    api.post<ApiResult<{ paymentUrl: string }>>('/billing/subscribe', data).then((r) => r.data.data),

  getSubscription: () =>
    api.get<ApiResult<SubscriptionDto>>('/billing/subscription').then((r) => r.data.data),

  cancelSubscription: () =>
    api.post('/billing/subscription/cancel').then((r) => r.data),

  upgrade: (data: UpgradeRequest) =>
    api.post<ApiResult<SubscriptionDto>>('/billing/subscription/upgrade', data).then((r) => r.data.data),

  downgrade: (data: UpgradeRequest) =>
    api.post<ApiResult<SubscriptionDto>>('/billing/subscription/downgrade', data).then((r) => r.data.data),

  getUsage: () =>
    api.get<ApiResult<UsageDto>>('/billing/usage').then((r) => r.data.data),

  getTransactions: () =>
    api.get<ApiResult<TransactionDto[]>>('/billing/transactions').then((r) => r.data.data),
};
