import type { BusinessType } from '../../types/content';

const businessTypes: { value: BusinessType; label: string }[] = [
  { value: 'SpazaShop', label: 'Spaza Shop' },
  { value: 'HairSalon', label: 'Hair Salon' },
  { value: 'Church', label: 'Church' },
  { value: 'Restaurant', label: 'Restaurant' },
  { value: 'Gym', label: 'Gym' },
  { value: 'FuneralParlor', label: 'Funeral Parlor' },
  { value: 'TaxiAssociation', label: 'Taxi Association' },
  { value: 'General', label: 'General Business' },
];

interface BusinessTypeSelectorProps {
  value: BusinessType;
  onChange: (value: BusinessType) => void;
}

export function BusinessTypeSelector({ value, onChange }: BusinessTypeSelectorProps) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-1">
        Business Type
      </label>
      <select
        value={value}
        onChange={(e) => onChange(e.target.value as BusinessType)}
        className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-gray-900 focus:border-primary-500 focus:ring-1 focus:ring-primary-500 focus:outline-none"
      >
        {businessTypes.map((bt) => (
          <option key={bt.value} value={bt.value}>
            {bt.label}
          </option>
        ))}
      </select>
    </div>
  );
}
