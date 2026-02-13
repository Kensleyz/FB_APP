import { useState } from 'react';
import { Sparkles } from 'lucide-react';
import { useContentStore } from '../../store/contentStore';
import { useContent } from '../../hooks/useContent';
import { BusinessTypeSelector } from './BusinessTypeSelector';
import { ToneSelector } from './ToneSelector';
import { PostTypeSelector } from './PostTypeSelector';
import { PostVariations } from './PostVariations';
import { Input } from '../common/Input';
import { Button } from '../common/Button';
import { Alert } from '../common/Alert';
import type {  Language, PostType, PostVariation, Tone } from '../../types/content';

interface ContentCreatorFormProps {
  onSchedule: (variation: PostVariation) => void;
}

export function ContentCreatorForm({ onSchedule }: ContentCreatorFormProps) {
  const {
    businessName,
    businessDescription,
    businessType,
    tone,
    postType,
    language,
    variations,
    generating,
    setFormField,
    updateVariation,
  } = useContentStore();

  const { generatePost } = useContent();
  const [error, setError] = useState<string | null>(null);

  const handleGenerate = async () => {
    setError(null);
    if (!businessName.trim()) {
      setError('Please enter your business name.');
      return;
    }
    try {
      await generatePost({
        businessName,
        businessDescription,
        businessType,
        tone,
        postType,
        language,
      });
    } catch (err) {
      setError((err as Error).message);
    }
  };

  return (
    <div className="space-y-6">
      {error && <Alert type="error">{error}</Alert>}

      <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-5 space-y-5">
        <h3 className="text-lg font-semibold text-gray-900">Business Details</h3>

        <Input
          label="Business Name"
          placeholder="e.g., Mama Joy's Kitchen"
          value={businessName}
          onChange={(e) => setFormField('businessName', e.target.value)}
        />

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Business Description (optional)
          </label>
          <textarea
            placeholder="Tell us about your business, what you sell or offer..."
            value={businessDescription}
            onChange={(e) => setFormField('businessDescription', e.target.value)}
            className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-gray-900 placeholder-gray-400 focus:border-primary-500 focus:ring-1 focus:ring-primary-500 focus:outline-none resize-none"
            rows={3}
          />
        </div>

        <BusinessTypeSelector
          value={businessType}
          onChange={(v) => setFormField('businessType', v)}
        />

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Language</label>
          <select
            value={language}
            onChange={(e) => setFormField('language', e.target.value as Language)}
            className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-gray-900 focus:border-primary-500 focus:ring-1 focus:ring-primary-500 focus:outline-none"
          >
            <option value="English">English</option>
            <option value="Afrikaans">Afrikaans</option>
            <option value="Mixed">Mixed (English + Afrikaans)</option>
          </select>
        </div>
      </div>

      <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-5 space-y-5">
        <h3 className="text-lg font-semibold text-gray-900">Post Settings</h3>
        <ToneSelector
          value={tone}
          onChange={(v: Tone) => setFormField('tone', v)}
        />
        <PostTypeSelector
          value={postType}
          onChange={(v: PostType) => setFormField('postType', v)}
        />
      </div>

      <Button
        onClick={handleGenerate}
        loading={generating}
        size="lg"
        className="w-full"
      >
        <Sparkles className="w-5 h-5 mr-2" />
        Generate Content
      </Button>

      <PostVariations
        variations={variations}
        onSchedule={onSchedule}
        onEdit={updateVariation}
      />
    </div>
  );
}
