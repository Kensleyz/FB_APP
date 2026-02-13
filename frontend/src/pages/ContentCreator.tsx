import { useState } from 'react';
import { ContentCreatorForm } from '../components/content/ContentCreator';
import { Modal } from '../components/common/Modal';
import { ScheduleForm } from '../components/scheduler/ScheduleForm';
import { scheduleService } from '../services/dashboardService';
import { Alert } from '../components/common/Alert';
import type { PostVariation } from '../types/content';

export function ContentCreatorPage() {
  const [scheduleModal, setScheduleModal] = useState(false);
  const [selectedVariation, setSelectedVariation] = useState<PostVariation | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const handleSchedule = (variation: PostVariation) => {
    setSelectedVariation(variation);
    setScheduleModal(true);
  };

  return (
    <div className="max-w-3xl mx-auto space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Create Content</h1>
        <p className="text-sm text-gray-500 mt-1">
          Generate AI-powered Facebook posts tailored for your business.
        </p>
      </div>

      {success && (
        <Alert type="success">{success}</Alert>
      )}

      <ContentCreatorForm onSchedule={handleSchedule} />

      <Modal
        isOpen={scheduleModal}
        onClose={() => setScheduleModal(false)}
        title="Schedule Post"
        size="lg"
      >
        {selectedVariation && (
          <ScheduleForm
            initialContent={`${selectedVariation.content}\n\n${selectedVariation.hashtags.map((h) => `#${h}`).join(' ')}`}
            initialHashtags={selectedVariation.hashtags}
            initialCta={selectedVariation.callToAction}
            onSubmit={async (data) => {
              await scheduleService.create(data);
              setScheduleModal(false);
              setSuccess('Post scheduled successfully!');
              setTimeout(() => setSuccess(null), 5000);
            }}
          />
        )}
      </Modal>
    </div>
  );
}
