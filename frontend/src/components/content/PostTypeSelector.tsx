import type { PostType } from '../../types/content';
import { Megaphone, GraduationCap, MessageCircle, CalendarDays, ShoppingBag, Users } from 'lucide-react';

const postTypes: { value: PostType; label: string; icon: typeof Megaphone }[] = [
  { value: 'Promotional', label: 'Promotional', icon: Megaphone },
  { value: 'Educational', label: 'Educational', icon: GraduationCap },
  { value: 'Engagement', label: 'Engagement', icon: MessageCircle },
  { value: 'Event', label: 'Event', icon: CalendarDays },
  { value: 'ProductShowcase', label: 'Product Showcase', icon: ShoppingBag },
  { value: 'Community', label: 'Community', icon: Users },
];

interface PostTypeSelectorProps {
  value: PostType;
  onChange: (value: PostType) => void;
}

export function PostTypeSelector({ value, onChange }: PostTypeSelectorProps) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-2">Post Type</label>
      <div className="grid grid-cols-2 sm:grid-cols-3 gap-2">
        {postTypes.map((pt) => {
          const Icon = pt.icon;
          return (
            <button
              key={pt.value}
              type="button"
              onClick={() => onChange(pt.value)}
              className={`flex items-center gap-2 p-3 rounded-lg border transition-colors ${
                value === pt.value
                  ? 'border-primary-500 bg-primary-50 text-primary-700'
                  : 'border-gray-200 hover:border-gray-300 text-gray-700'
              }`}
            >
              <Icon className="w-4 h-4" />
              <span className="text-sm font-medium">{pt.label}</span>
            </button>
          );
        })}
      </div>
    </div>
  );
}
