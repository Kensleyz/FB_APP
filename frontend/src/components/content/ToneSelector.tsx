import type { Tone } from '../../types/content';

const tones: { value: Tone; label: string; description: string }[] = [
  { value: 'Professional', label: 'Professional', description: 'Formal and business-like' },
  { value: 'Casual', label: 'Casual', description: 'Relaxed and conversational' },
  { value: 'Friendly', label: 'Friendly', description: 'Warm and approachable' },
  { value: 'Energetic', label: 'Energetic', description: 'Exciting and dynamic' },
  { value: 'Respectful', label: 'Respectful', description: 'Polite and considerate' },
];

interface ToneSelectorProps {
  value: Tone;
  onChange: (value: Tone) => void;
}

export function ToneSelector({ value, onChange }: ToneSelectorProps) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-2">Tone</label>
      <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-2">
        {tones.map((tone) => (
          <button
            key={tone.value}
            type="button"
            onClick={() => onChange(tone.value)}
            className={`p-3 rounded-lg border text-left transition-colors ${
              value === tone.value
                ? 'border-primary-500 bg-primary-50 text-primary-700'
                : 'border-gray-200 hover:border-gray-300 text-gray-700'
            }`}
          >
            <p className="text-sm font-medium">{tone.label}</p>
            <p className="text-xs opacity-70 mt-0.5">{tone.description}</p>
          </button>
        ))}
      </div>
    </div>
  );
}
