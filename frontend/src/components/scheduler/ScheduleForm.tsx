import { useState, useEffect } from 'react';
import { Button } from '../common/Button';
import { Input } from '../common/Input';
import { Alert } from '../common/Alert';
import { useFacebookStore } from '../../store/facebookStore';
import type { CreateScheduleRequest } from '../../types/dashboard';

interface ScheduleFormProps {
  initialContent?: string;
  initialHashtags?: string[];
  initialCta?: string;
  onSubmit: (data: CreateScheduleRequest) => Promise<void>;
  loading?: boolean;
}

export function ScheduleForm({
  initialContent = '',
  initialHashtags = [],
  initialCta = '',
  onSubmit,
  loading = false,
}: ScheduleFormProps) {
  const pages = useFacebookStore((s) => s.pages);
  const [pageId, setPageId] = useState(pages[0]?.id || '');

  useEffect(() => {
    if (pages.length > 0 && !pageId) {
      setPageId(pages[0].id);
    }
  }, [pages, pageId]);
  const [content, setContent] = useState(initialContent);
  const [hashtags, setHashtags] = useState(initialHashtags.join(', '));
  const [callToAction, setCallToAction] = useState(initialCta);
  const [scheduledFor, setScheduledFor] = useState('');
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!pageId) {
      setError('Please select a Facebook page.');
      return;
    }
    if (!content.trim()) {
      setError('Please enter post content.');
      return;
    }
    if (!scheduledFor) {
      setError('Please select a date and time.');
      return;
    }

    await onSubmit({
      pageId,
      content,
      hashtags: hashtags.split(',').map((h) => h.trim()).filter(Boolean),
      callToAction,
      scheduledFor: new Date(scheduledFor).toISOString(),
    });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {error && <Alert type="error">{error}</Alert>}

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">Facebook Page</label>
        <select
          value={pageId}
          onChange={(e) => setPageId(e.target.value)}
          className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-gray-900 focus:border-primary-500 focus:ring-1 focus:ring-primary-500 focus:outline-none"
        >
          <option value="">Select a page</option>
          {pages.map((p) => (
            <option key={p.id} value={p.id}>{p.pageName}</option>
          ))}
        </select>
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">Content</label>
        <textarea
          value={content}
          onChange={(e) => setContent(e.target.value)}
          placeholder="Write your post content..."
          className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-gray-900 placeholder-gray-400 focus:border-primary-500 focus:ring-1 focus:ring-primary-500 focus:outline-none resize-none"
          rows={4}
        />
        <p className="text-xs text-gray-400 mt-1">{content.length}/280 characters</p>
      </div>

      <Input
        label="Hashtags"
        placeholder="e.g., spazashop, mzansi, small business"
        value={hashtags}
        onChange={(e) => setHashtags(e.target.value)}
      />

      <Input
        label="Call to Action"
        placeholder="e.g., Visit us today!"
        value={callToAction}
        onChange={(e) => setCallToAction(e.target.value)}
      />

      <Input
        label="Schedule For"
        type="datetime-local"
        value={scheduledFor}
        onChange={(e) => setScheduledFor(e.target.value)}
      />

      <Button type="submit" loading={loading} className="w-full">
        Schedule Post
      </Button>
    </form>
  );
}
