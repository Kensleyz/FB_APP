import { FileText, Image, Calendar, TrendingUp } from 'lucide-react';
import type { DashboardOverview } from '../../types/dashboard';

interface OverviewCardsProps {
  data: DashboardOverview;
}

export function OverviewCards({ data }: OverviewCardsProps) {
  const cards = [
    {
      label: 'Posts This Month',
      value: `${data.postsThisMonth} / ${data.postsLimit}`,
      icon: FileText,
      color: 'text-primary-600 bg-primary-50',
    },
    {
      label: 'Images Created',
      value: `${data.imagesThisMonth} / ${data.imagesLimit}`,
      icon: Image,
      color: 'text-green-600 bg-green-50',
    },
    {
      label: 'Upcoming Posts',
      value: String(data.upcomingPosts),
      icon: Calendar,
      color: 'text-amber-600 bg-amber-50',
    },
    {
      label: 'Engagement Rate',
      value: `${data.engagementRate.toFixed(1)}%`,
      icon: TrendingUp,
      color: 'text-purple-600 bg-purple-50',
    },
  ];

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      {cards.map((card) => (
        <div
          key={card.label}
          className="bg-white rounded-xl border border-gray-200 shadow-sm p-5"
        >
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-500">{card.label}</p>
              <p className="text-2xl font-bold text-gray-900 mt-1">{card.value}</p>
            </div>
            <div className={`p-3 rounded-lg ${card.color}`}>
              <card.icon className="w-5 h-5" />
            </div>
          </div>
        </div>
      ))}
    </div>
  );
}
