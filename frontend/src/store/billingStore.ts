import { create } from 'zustand';
import type { PlanDto, SubscriptionDto, UsageDto, TransactionDto } from '../types/billing';

interface BillingState {
  plans: PlanDto[];
  subscription: SubscriptionDto | null;
  usage: UsageDto | null;
  transactions: TransactionDto[];
  loading: boolean;
  setPlans: (plans: PlanDto[]) => void;
  setSubscription: (subscription: SubscriptionDto | null) => void;
  setUsage: (usage: UsageDto) => void;
  setTransactions: (transactions: TransactionDto[]) => void;
  setLoading: (loading: boolean) => void;
}

export const useBillingStore = create<BillingState>()((set) => ({
  plans: [],
  subscription: null,
  usage: null,
  transactions: [],
  loading: false,

  setPlans: (plans) => set({ plans }),
  setSubscription: (subscription) => set({ subscription }),
  setUsage: (usage) => set({ usage }),
  setTransactions: (transactions) => set({ transactions }),
  setLoading: (loading) => set({ loading }),
}));
