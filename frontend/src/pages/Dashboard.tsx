import { useEffect, useState } from 'react';
import { OverviewCards } from '../components/dashboard/OverviewCards';
import { UsageMeter } from '../components/dashboard/UsageMeter';
import { RecentPosts } from '../components/dashboard/RecentPosts';
import { QuickActions } from '../components/dashboard/QuickActions';
import { Loading } from '../components/common/Loading';
import { Alert } from '../components/common/Alert';
import { dashboardService } from '../services/dashboardService';
import type { DashboardOverview } from '../types/dashboard';

export function Dashboard() {
  const [overview, setOverview] = useState<DashboardOverview | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    dashboardService
      .getOverview()
      .then(setOverview)
      .catch(() => setError('Failed to load dashboard data.'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <Loading message="Loading dashboard..." />;

  if (error) {
    return (
      <div className="max-w-4xl mx-auto">
        <Alert type="error">{error}</Alert>
      </div>
    );
  }

  if (!overview) return null;

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-sm text-gray-500 mt-1">Welcome back! Here's your page overview.</p>
      </div>

      <OverviewCards data={overview} />

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2">
          <RecentPosts posts={overview.recentPosts} />
        </div>
        <div className="space-y-6">
          <UsageMeter
            postsUsed={overview.postsThisMonth}
            postsLimit={overview.postsLimit}
            imagesUsed={overview.imagesThisMonth}
            imagesLimit={overview.imagesLimit}
            pagesUsed={overview.connectedPages}
            pagesLimit={overview.totalPagesAllowed}
          />
          <QuickActions />
        </div>
      </div>
    </div>
  );
}
