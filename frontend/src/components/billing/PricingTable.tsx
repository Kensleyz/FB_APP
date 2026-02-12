import { Check } from 'lucide-react';
import { Button } from '../common/Button';
import { formatCurrency } from '../../utils/formatters';
import type { SubscriptionTier } from '../../types/auth';

interface PricingPlan {
  tier: SubscriptionTier;
  name: string;
  price: number;
  description: string;
  features: string[];
  popular?: boolean;
}

const plans: PricingPlan[] = [
  {
    tier: 'Free',
    name: 'Free',
    price: 0,
    description: 'Get started with the basics',
    features: [
      '5 posts per month',
      '1 Facebook page',
      '2 images per month',
      'Basic AI content',
    ],
  },
  {
    tier: 'Starter',
    name: 'Starter',
    price: 30,
    description: 'For growing businesses',
    features: [
      '10 posts per month',
      '1 Facebook page',
      '5 images per month',
      'All AI content types',
      'Post scheduling',
    ],
  },
  {
    tier: 'Growth',
    name: 'Growth',
    price: 50,
    description: 'Most popular for active businesses',
    features: [
      '30 posts per month',
      '3 Facebook pages',
      '15 images per month',
      'All AI content types',
      'Post scheduling',
      'Analytics dashboard',
      '7-day free trial',
    ],
    popular: true,
  },
  {
    tier: 'Pro',
    name: 'Pro',
    price: 150,
    description: 'For agencies and power users',
    features: [
      '100 posts per month',
      '10 Facebook pages',
      '50 images per month',
      'All AI content types',
      'Advanced scheduling',
      'Full analytics',
      'Priority support',
    ],
  },
];

interface PricingTableProps {
  currentTier?: SubscriptionTier;
  onSelectPlan: (tier: SubscriptionTier) => void;
  loading?: boolean;
}

export function PricingTable({ currentTier, onSelectPlan, loading }: PricingTableProps) {
  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      {plans.map((plan) => {
        const isCurrent = currentTier === plan.tier;
        return (
          <div
            key={plan.tier}
            className={`relative bg-white rounded-xl border shadow-sm p-6 flex flex-col ${
              plan.popular
                ? 'border-primary-500 ring-2 ring-primary-500'
                : 'border-gray-200'
            }`}
          >
            {plan.popular && (
              <span className="absolute -top-3 left-1/2 -translate-x-1/2 bg-primary-600 text-white text-xs font-bold px-3 py-1 rounded-full">
                Most Popular
              </span>
            )}

            <h3 className="text-lg font-bold text-gray-900">{plan.name}</h3>
            <p className="text-sm text-gray-500 mt-1">{plan.description}</p>

            <div className="mt-4 mb-6">
              <span className="text-3xl font-bold text-gray-900">
                {plan.price === 0 ? 'Free' : formatCurrency(plan.price)}
              </span>
              {plan.price > 0 && (
                <span className="text-sm text-gray-500">/month</span>
              )}
            </div>

            <ul className="space-y-2.5 flex-1 mb-6">
              {plan.features.map((f) => (
                <li key={f} className="flex items-start gap-2 text-sm text-gray-600">
                  <Check className="w-4 h-4 text-green-500 mt-0.5 shrink-0" />
                  {f}
                </li>
              ))}
            </ul>

            <Button
              variant={isCurrent ? 'secondary' : plan.popular ? 'primary' : 'outline'}
              disabled={isCurrent}
              loading={loading}
              onClick={() => onSelectPlan(plan.tier)}
              className="w-full"
            >
              {isCurrent ? 'Current Plan' : plan.price === 0 ? 'Get Started' : 'Subscribe'}
            </Button>
          </div>
        );
      })}
    </div>
  );
}
