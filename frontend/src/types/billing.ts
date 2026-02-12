import type { SubscriptionTier } from './auth';

export interface PlanDto {
  tier: SubscriptionTier;
  name: string;
  price: number;
  currency: string;
  postsPerMonth: number;
  pagesAllowed: number;
  imagesPerMonth: number;
  features: string[];
}

export interface SubscriptionDto {
  id: string;
  tier: SubscriptionTier;
  status: SubscriptionStatus;
  amount: number;
  currency: string;
  startDate: string;
  nextBillingDate?: string;
  cancelledAt?: string;
}

export type SubscriptionStatus = 'Active' | 'Cancelled' | 'Suspended' | 'PastDue';

export interface UsageDto {
  period: string;
  postsGenerated: number;
  postsLimit: number;
  imagesCreated: number;
  imagesLimit: number;
  postsPublished: number;
  pagesConnected: number;
  pagesLimit: number;
}

export interface TransactionDto {
  id: string;
  amount: number;
  currency: string;
  status: TransactionStatus;
  paymentMethod: string;
  transactionType: string;
  createdAt: string;
  completedAt?: string;
}

export type TransactionStatus = 'Pending' | 'Complete' | 'Failed' | 'Refunded';

export interface SubscribeRequest {
  tier: SubscriptionTier;
}

export interface UpgradeRequest {
  newTier: SubscriptionTier;
}
