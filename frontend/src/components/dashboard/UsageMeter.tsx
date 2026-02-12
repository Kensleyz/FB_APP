import { getUsagePercentage } from '../../utils/formatters';

interface UsageMeterProps {
  label: string;
  used: number;
  limit: number;
}

function MeterBar({ label, used, limit }: UsageMeterProps) {
  const pct = getUsagePercentage(used, limit);
  const barColor =
    pct >= 90 ? 'bg-red-500' : pct >= 70 ? 'bg-amber-500' : 'bg-primary-600';

  return (
    <div>
      <div className="flex items-center justify-between mb-1">
        <span className="text-sm text-gray-600">{label}</span>
        <span className="text-sm font-medium text-gray-900">
          {used} / {limit}
        </span>
      </div>
      <div className="w-full h-2 bg-gray-100 rounded-full overflow-hidden">
        <div
          className={`h-full rounded-full transition-all ${barColor}`}
          style={{ width: `${pct}%` }}
        />
      </div>
    </div>
  );
}

interface UsageMeterGroupProps {
  postsUsed: number;
  postsLimit: number;
  imagesUsed: number;
  imagesLimit: number;
  pagesUsed: number;
  pagesLimit: number;
}

export function UsageMeter({
  postsUsed,
  postsLimit,
  imagesUsed,
  imagesLimit,
  pagesUsed,
  pagesLimit,
}: UsageMeterGroupProps) {
  return (
    <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-5 space-y-4">
      <h3 className="text-lg font-semibold text-gray-900">Usage This Month</h3>
      <MeterBar label="Posts Generated" used={postsUsed} limit={postsLimit} />
      <MeterBar label="Images Created" used={imagesUsed} limit={imagesLimit} />
      <MeterBar label="Connected Pages" used={pagesUsed} limit={pagesLimit} />
    </div>
  );
}
