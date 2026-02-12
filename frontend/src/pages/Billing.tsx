import { useEffect, useState } from 'react';
import { useBilling } from '../hooks/useBilling';
import { useBillingStore } from '../store/billingStore';
import { useAuthStore } from '../store/authStore';
import { CurrentPlan } from '../components/billing/CurrentPlan';
import { UsageBreakdown } from '../components/billing/UsageBreakdown';
import { PricingTable } from '../components/billing/PricingTable';
import { PaymentHistory } from '../components/billing/PaymentHistory';
import { Loading } from '../components/common/Loading';
import { Alert } from '../components/common/Alert';
import { Modal } from '../components/common/Modal';
import type { SubscriptionTier } from '../types/auth';

export function Billing() {
  const user = useAuthStore((s) => s.user);
  const { subscription, usage, transactions, loading } = useBillingStore();
  const {
    fetchPlans,
    fetchSubscription,
    fetchUsage,
    fetchTransactions,
    subscribe,
    cancelSubscription,
    upgrade,
    error,
  } = useBilling();
  const [showPlans, setShowPlans] = useState(false);
  const [confirmCancel, setConfirmCancel] = useState(false);

  useEffect(() => {
    fetchPlans();
    fetchSubscription();
    fetchUsage();
    fetchTransactions();
  }, [fetchPlans, fetchSubscription, fetchUsage, fetchTransactions]);

  const handleSelectPlan = (tier: SubscriptionTier) => {
    if (subscription) {
      upgrade(tier);
    } else {
      subscribe(tier);
    }
    setShowPlans(false);
  };

  const handleCancel = () => {
    cancelSubscription();
    setConfirmCancel(false);
  };

  if (loading) return <Loading message="Loading billing..." />;

  return (
    <div className="space-y-6 max-w-4xl">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Billing</h1>
        <p className="text-sm text-gray-500 mt-1">Manage your subscription and payments.</p>
      </div>

      {error && <Alert type="error">{error}</Alert>}

      <CurrentPlan
        subscription={subscription}
        onCancel={() => setConfirmCancel(true)}
        onUpgrade={() => setShowPlans(true)}
      />

      {usage && <UsageBreakdown usage={usage} />}

      <PaymentHistory transactions={transactions} />

      {/* Change plan modal */}
      <Modal
        isOpen={showPlans}
        onClose={() => setShowPlans(false)}
        title="Choose a Plan"
        size="lg"
      >
        <div className="py-4">
          <PricingTable
            currentTier={user?.subscriptionTier}
            onSelectPlan={handleSelectPlan}
          />
        </div>
      </Modal>

      {/* Cancel confirmation */}
      <Modal
        isOpen={confirmCancel}
        onClose={() => setConfirmCancel(false)}
        title="Cancel Subscription"
        size="sm"
      >
        <div className="space-y-4">
          <p className="text-sm text-gray-600">
            Are you sure you want to cancel your subscription? You'll lose access to premium features at the end of your billing period.
          </p>
          <div className="flex gap-2 justify-end">
            <button
              onClick={() => setConfirmCancel(false)}
              className="px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 rounded-lg"
            >
              Keep Plan
            </button>
            <button
              onClick={handleCancel}
              className="px-4 py-2 text-sm font-medium text-white bg-red-600 hover:bg-red-700 rounded-lg"
            >
              Yes, Cancel
            </button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
