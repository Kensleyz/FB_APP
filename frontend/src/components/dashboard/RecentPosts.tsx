import { CheckCircle2, Clock, XCircle, Ban } from 'lucide-react';
import type { RecentPostDto } from '../../types/dashboard';
import { formatRelativeTime, truncate } from '../../utils/formatters';

interface RecentPostsProps {
  posts: RecentPostDto[];
}

const statusConfig = {
  Published: { icon: CheckCircle2, color: 'text-green-600', label: 'Published' },
  Scheduled: { icon: Clock, color: 'text-amber-600', label: 'Scheduled' },
  Failed: { icon: XCircle, color: 'text-red-600', label: 'Failed' },
  Cancelled: { icon: Ban, color: 'text-gray-400', label: 'Cancelled' },
};

export function RecentPosts({ posts }: RecentPostsProps) {
  if (posts.length === 0) {
    return (
      <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-5">
        <h3 className="text-lg font-semibold text-gray-900 mb-4">Recent Posts</h3>
        <p className="text-sm text-gray-500 text-center py-6">
          No posts yet. Create your first post!
        </p>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-5">
      <h3 className="text-lg font-semibold text-gray-900 mb-4">Recent Posts</h3>
      <div className="space-y-3">
        {posts.map((post) => {
          const status = statusConfig[post.status];
          const StatusIcon = status.icon;
          const dateStr = post.publishedAt || post.scheduledFor;

          return (
            <div
              key={post.id}
              className="flex items-start gap-3 p-3 rounded-lg hover:bg-gray-50 transition-colors"
            >
              <StatusIcon className={`w-5 h-5 mt-0.5 shrink-0 ${status.color}`} />
              <div className="flex-1 min-w-0">
                <p className="text-sm text-gray-900">{truncate(post.content, 100)}</p>
                <div className="flex items-center gap-3 mt-1">
                  <span className="text-xs text-gray-500">{post.pageName}</span>
                  {dateStr && (
                    <span className="text-xs text-gray-400">
                      {formatRelativeTime(dateStr)}
                    </span>
                  )}
                </div>
              </div>
              {post.status === 'Published' && (
                <div className="flex items-center gap-3 text-xs text-gray-500 shrink-0">
                  {post.likes != null && <span>{post.likes} likes</span>}
                  {post.comments != null && <span>{post.comments} comments</span>}
                </div>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}
