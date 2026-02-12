import { Clock, Edit2, Trash2, Send } from 'lucide-react';
import type { ScheduleDto } from '../../types/dashboard';
import { formatDateTime } from '../../utils/formatters';
import { Button } from '../common/Button';

interface PostPreviewProps {
  post: ScheduleDto;
  onEdit: (post: ScheduleDto) => void;
  onDelete: (id: string) => void;
  onPublishNow: (id: string) => void;
}

const statusBadge = {
  Scheduled: 'bg-amber-100 text-amber-700',
  Published: 'bg-green-100 text-green-700',
  Failed: 'bg-red-100 text-red-700',
  Cancelled: 'bg-gray-100 text-gray-500',
};

export function PostPreview({ post, onEdit, onDelete, onPublishNow }: PostPreviewProps) {
  return (
    <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-5">
      <div className="flex items-center justify-between mb-3">
        <div className="flex items-center gap-2">
          <span className="text-sm font-medium text-gray-900">{post.pageName}</span>
          <span className={`text-xs px-2 py-0.5 rounded-full font-medium ${statusBadge[post.status]}`}>
            {post.status}
          </span>
        </div>
        <div className="flex items-center gap-1">
          {post.status === 'Scheduled' && (
            <>
              <button
                onClick={() => onEdit(post)}
                className="p-1.5 text-gray-400 hover:text-gray-600 rounded-lg hover:bg-gray-100"
              >
                <Edit2 className="w-4 h-4" />
              </button>
              <button
                onClick={() => onDelete(post.id)}
                className="p-1.5 text-gray-400 hover:text-red-600 rounded-lg hover:bg-red-50"
              >
                <Trash2 className="w-4 h-4" />
              </button>
            </>
          )}
        </div>
      </div>

      <p className="text-sm text-gray-900">{post.content}</p>

      {post.hashtags.length > 0 && (
        <div className="flex flex-wrap gap-1.5 mt-2">
          {post.hashtags.map((tag) => (
            <span key={tag} className="text-xs text-primary-600 font-medium">
              #{tag}
            </span>
          ))}
        </div>
      )}

      {post.callToAction && (
        <p className="text-xs text-gray-500 mt-2">CTA: {post.callToAction}</p>
      )}

      <div className="flex items-center justify-between mt-4 pt-3 border-t border-gray-100">
        <div className="flex items-center gap-1.5 text-xs text-gray-500">
          <Clock className="w-3.5 h-3.5" />
          {formatDateTime(post.scheduledFor)}
        </div>
        {post.status === 'Scheduled' && (
          <Button size="sm" variant="outline" onClick={() => onPublishNow(post.id)}>
            <Send className="w-3.5 h-3.5 mr-1" />
            Publish Now
          </Button>
        )}
      </div>

      {post.errorMessage && (
        <p className="text-xs text-red-600 mt-2">{post.errorMessage}</p>
      )}
    </div>
  );
}
