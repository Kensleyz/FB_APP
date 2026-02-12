import type { UsageDto } from '../../types/billing';
import { getUsagePercentage } from '../../utils/formatters';

interface UsageBreakdownProps {
  usage: UsageDto;
}

function UsageRow({ label, used, limit }: { label: string; used: number; limit: number }) {
  const pct = getUsagePercentage(used, limit);
  const barColor =
    pct >= 90 ? 'bg-red-500' : pct >= 70 ? 'bg-amber-500' : 'bg-primary-600';

  return (
    <div className="space-y-1.5">
      <div className="flex justify-between text-sm">
        <span className="text-gray-600">{label}</span>
        <span className="font-medium text-gray-900">
          {used} / {limit} ({pct}%)
        </span>
      </div>
      <div className="w-full h-2 bg-gray-100 rounded-full overflow-hidden">
        <div className={`h-full rounded-full transition-all ${barColor}`} style={{ width: `${pct}%` }} />
      </div>
    </div>
  );
}

export function UsageBreakdown({ usage }: UsageBreakdownProps) {
  return (
    <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-6">
      <h3 className="text-lg font-semibold text-gray-900 mb-4">
        Usage - {usage.period}
      </h3>
      <div className="space-y-4">
        <UsageRow label="Posts Generated" used={usage.postsGenerated} limit={usage.postsLimit} />
        <UsageRow label="Images Created" used={usage.imagesCreated} limit={usage.imagesLimit} />
        <UsageRow label="Connected Pages" used={usage.pagesConnected} limit={usage.pagesLimit} />
      </div>
    </div>
  );
}
