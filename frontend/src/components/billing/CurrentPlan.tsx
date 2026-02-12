import { CreditCard, AlertTriangle } from 'lucide-react';
import type { SubscriptionDto } from '../../types/billing';
import { formatCurrency, formatDate } from '../../utils/formatters';
import { Button } from '../common/Button';

interface CurrentPlanProps {
  subscription: SubscriptionDto | null;
  onCancel: () => void;
  onUpgrade: () => void;
}

export function CurrentPlan({ subscription, onCancel, onUpgrade }: CurrentPlanProps) {
  if (!subscription) {
    return (
      <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-6">
        <div className="flex items-center gap-3 mb-4">
          <div className="p-2 bg-gray-100 rounded-lg">
            <CreditCard className="w-5 h-5 text-gray-600" />
          </div>
          <div>
            <h3 className="text-lg font-semibold text-gray-900">Free Plan</h3>
            <p className="text-sm text-gray-500">You are on the free tier</p>
          </div>
        </div>
        <Button onClick={onUpgrade}>Upgrade Now</Button>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-6">
      <div className="flex items-center justify-between mb-4">
        <div className="flex items-center gap-3">
          <div className="p-2 bg-primary-50 rounded-lg">
            <CreditCard className="w-5 h-5 text-primary-600" />
          </div>
          <div>
            <h3 className="text-lg font-semibold text-gray-900">
              {subscription.tier} Plan
            </h3>
            <p className="text-sm text-gray-500">
              {formatCurrency(subscription.amount)}/month
            </p>
          </div>
        </div>
        <span
          className={`text-xs px-2 py-1 rounded-full font-medium ${
            subscription.status === 'Active'
              ? 'bg-green-100 text-green-700'
              : subscription.status === 'Cancelled'
              ? 'bg-red-100 text-red-700'
              : 'bg-amber-100 text-amber-700'
          }`}
        >
          {subscription.status}
        </span>
      </div>

      {subscription.status === 'PastDue' && (
        <div className="flex items-center gap-2 p-3 bg-amber-50 border border-amber-200 rounded-lg mb-4">
          <AlertTriangle className="w-4 h-4 text-amber-600" />
          <p className="text-sm text-amber-700">Payment is past due. Please update your payment method.</p>
        </div>
      )}

      <div className="grid grid-cols-2 gap-4 text-sm mb-4">
        <div>
          <p className="text-gray-500">Start Date</p>
          <p className="font-medium text-gray-900">{formatDate(subscription.startDate)}</p>
        </div>
        {subscription.nextBillingDate && (
          <div>
            <p className="text-gray-500">Next Billing</p>
            <p className="font-medium text-gray-900">{formatDate(subscription.nextBillingDate)}</p>
          </div>
        )}
      </div>

      <div className="flex gap-2">
        <Button onClick={onUpgrade} size="sm">Change Plan</Button>
        {subscription.status === 'Active' && (
          <Button onClick={onCancel} variant="outline" size="sm">Cancel</Button>
        )}
      </div>
    </div>
  );
}
