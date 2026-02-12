import { useState } from 'react';
import { Check, Copy, Calendar, Send } from 'lucide-react';
import type { PostVariation } from '../../types/content';
import { Button } from '../common/Button';

interface PostVariationsProps {
  variations: PostVariation[];
  onSchedule: (variation: PostVariation) => void;
  onEdit: (id: string, content: string) => void;
}

export function PostVariations({ variations, onSchedule, onEdit }: PostVariationsProps) {
  const [copiedId, setCopiedId] = useState<string | null>(null);

  const copyToClipboard = async (variation: PostVariation) => {
    const text = `${variation.content}\n\n${variation.hashtags.map((h) => `#${h}`).join(' ')}\n\n${variation.callToAction}`;
    await navigator.clipboard.writeText(text);
    setCopiedId(variation.id);
    setTimeout(() => setCopiedId(null), 2000);
  };

  if (variations.length === 0) return null;

  return (
    <div className="space-y-4">
      <h3 className="text-lg font-semibold text-gray-900">Generated Variations</h3>
      {variations.map((variation, idx) => (
        <div
          key={variation.id}
          className="bg-white rounded-xl border border-gray-200 shadow-sm p-5"
        >
          <div className="flex items-center justify-between mb-3">
            <span className="text-sm font-medium text-gray-500">
              Variation {idx + 1}
            </span>
            <div className="flex items-center gap-2">
              <button
                onClick={() => copyToClipboard(variation)}
                className="p-1.5 text-gray-400 hover:text-gray-600 rounded-lg hover:bg-gray-100"
                title="Copy to clipboard"
              >
                {copiedId === variation.id ? (
                  <Check className="w-4 h-4 text-green-600" />
                ) : (
                  <Copy className="w-4 h-4" />
                )}
              </button>
            </div>
          </div>

          <textarea
            value={variation.content}
            onChange={(e) => onEdit(variation.id, e.target.value)}
            className="w-full p-3 border border-gray-200 rounded-lg text-sm text-gray-900 focus:border-primary-500 focus:ring-1 focus:ring-primary-500 focus:outline-none resize-none"
            rows={3}
          />

          <div className="flex flex-wrap gap-2 mt-3">
            {variation.hashtags.map((tag) => (
              <span
                key={tag}
                className="px-2 py-1 bg-primary-50 text-primary-700 text-xs rounded-full font-medium"
              >
                #{tag}
              </span>
            ))}
          </div>

          {variation.callToAction && (
            <p className="text-sm text-gray-600 mt-2">
              <span className="font-medium">CTA:</span> {variation.callToAction}
            </p>
          )}

          <div className="flex gap-2 mt-4">
            <Button
              size="sm"
              onClick={() => onSchedule(variation)}
            >
              <Calendar className="w-4 h-4 mr-1.5" />
              Schedule
            </Button>
            <Button
              size="sm"
              variant="outline"
              onClick={() => onSchedule(variation)}
            >
              <Send className="w-4 h-4 mr-1.5" />
              Publish Now
            </Button>
          </div>
        </div>
      ))}
    </div>
  );
}
