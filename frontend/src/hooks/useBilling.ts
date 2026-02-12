import { useState, useCallback } from 'react';
import { useBillingStore } from '../store/billingStore';
import { billingService } from '../services/billingService';
import type { SubscriptionTier } from '../types/auth';

export function useBilling() {
  const { setPlans, setSubscription, setUsage, setTransactions, setLoading } =
    useBillingStore();
  const [error, setError] = useState<string | null>(null);

  const fetchPlans = useCallback(async () => {
    setLoading(true);
    try {
      const plans = await billingService.getPlans();
      setPlans(plans);
    } catch {
      setError('Failed to fetch plans.');
    } finally {
      setLoading(false);
    }
  }, [setPlans, setLoading]);

  const fetchSubscription = useCallback(async () => {
    try {
      const sub = await billingService.getSubscription();
      setSubscription(sub);
    } catch {
      setSubscription(null);
    }
  }, [setSubscription]);

  const fetchUsage = useCallback(async () => {
    try {
      const usage = await billingService.getUsage();
      setUsage(usage);
    } catch {
      setError('Failed to fetch usage.');
    }
  }, [setUsage]);

  const fetchTransactions = useCallback(async () => {
    try {
      const txns = await billingService.getTransactions();
      setTransactions(txns);
    } catch {
      setError('Failed to fetch transactions.');
    }
  }, [setTransactions]);

  const subscribe = useCallback(async (tier: SubscriptionTier) => {
    try {
      const { paymentUrl } = await billingService.subscribe({ tier });
      window.location.href = paymentUrl;
    } catch {
      setError('Failed to create subscription.');
    }
  }, []);

  const cancelSubscription = useCallback(async () => {
    try {
      await billingService.cancelSubscription();
      await fetchSubscription();
    } catch {
      setError('Failed to cancel subscription.');
    }
  }, [fetchSubscription]);

  const upgrade = useCallback(
    async (newTier: SubscriptionTier) => {
      try {
        await billingService.upgrade({ newTier });
        await fetchSubscription();
      } catch {
        setError('Failed to upgrade subscription.');
      }
    },
    [fetchSubscription]
  );

  return {
    fetchPlans,
    fetchSubscription,
    fetchUsage,
    fetchTransactions,
    subscribe,
    cancelSubscription,
    upgrade,
    error,
    setError,
  };
}
