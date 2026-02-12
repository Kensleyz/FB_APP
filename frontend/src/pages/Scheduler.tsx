import { useEffect, useState, useCallback } from 'react';
import { Calendar } from '../components/scheduler/Calendar';
import { PostPreview } from '../components/scheduler/PostPreview';
import { ScheduleForm } from '../components/scheduler/ScheduleForm';
import { Modal } from '../components/common/Modal';
import { Loading } from '../components/common/Loading';
import { Alert } from '../components/common/Alert';
import { Button } from '../components/common/Button';
import { Plus } from 'lucide-react';
import { scheduleService } from '../services/dashboardService';
import type { ScheduleDto } from '../types/dashboard';

export function Scheduler() {
  const [month, setMonth] = useState(new Date());
  const [posts, setPosts] = useState<ScheduleDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [createModal, setCreateModal] = useState(false);
  const [creating, setCreating] = useState(false);

  const fetchPosts = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const monthStr = `${month.getFullYear()}-${String(month.getMonth() + 1).padStart(2, '0')}`;
      const data = await scheduleService.getCalendar(monthStr);
      setPosts(data.posts);
    } catch {
      setError('Failed to load scheduled posts.');
    } finally {
      setLoading(false);
    }
  }, [month]);

  useEffect(() => {
    fetchPosts();
  }, [fetchPosts]);

  const handleDelete = async (id: string) => {
    try {
      await scheduleService.delete(id);
      setPosts((prev) => prev.filter((p) => p.id !== id));
    } catch {
      setError('Failed to delete post.');
    }
  };

  const handlePublishNow = async (id: string) => {
    try {
      await scheduleService.publishNow(id);
      fetchPosts();
    } catch {
      setError('Failed to publish post.');
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Scheduler</h1>
          <p className="text-sm text-gray-500 mt-1">Plan and manage your content calendar.</p>
        </div>
        <Button onClick={() => setCreateModal(true)}>
          <Plus className="w-4 h-4 mr-2" />
          New Post
        </Button>
      </div>

      {error && <Alert type="error">{error}</Alert>}

      {loading ? (
        <Loading message="Loading calendar..." />
      ) : (
        <>
          <Calendar
            month={month}
            posts={posts}
            onMonthChange={setMonth}
            onDayClick={() => setCreateModal(true)}
          />

          {posts.length > 0 && (
            <div className="space-y-4">
              <h3 className="text-lg font-semibold text-gray-900">
                Scheduled Posts ({posts.filter((p) => p.status === 'Scheduled').length})
              </h3>
              {posts
                .filter((p) => p.status === 'Scheduled')
                .sort((a, b) => new Date(a.scheduledFor).getTime() - new Date(b.scheduledFor).getTime())
                .map((post) => (
                  <PostPreview
                    key={post.id}
                    post={post}
                    onEdit={() => {}}
                    onDelete={handleDelete}
                    onPublishNow={handlePublishNow}
                  />
                ))}
            </div>
          )}
        </>
      )}

      <Modal
        isOpen={createModal}
        onClose={() => setCreateModal(false)}
        title="Schedule New Post"
        size="lg"
      >
        <ScheduleForm
          loading={creating}
          onSubmit={async (data) => {
            setCreating(true);
            try {
              await scheduleService.create(data);
              setCreateModal(false);
              fetchPosts();
            } catch {
              setError('Failed to schedule post.');
            } finally {
              setCreating(false);
            }
          }}
        />
      </Modal>
    </div>
  );
}
