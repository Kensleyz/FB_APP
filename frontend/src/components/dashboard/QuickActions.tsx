import { useNavigate } from 'react-router-dom';
import { PenTool, CalendarDays, Facebook, TrendingUp } from 'lucide-react';

export function QuickActions() {
  const navigate = useNavigate();

  const actions = [
    {
      label: 'Create Post',
      description: 'Generate AI content',
      icon: PenTool,
      color: 'text-primary-600 bg-primary-50 hover:bg-primary-100',
      onClick: () => navigate('/create'),
    },
    {
      label: 'Schedule Post',
      description: 'Plan your content',
      icon: CalendarDays,
      color: 'text-green-600 bg-green-50 hover:bg-green-100',
      onClick: () => navigate('/scheduler'),
    },
    {
      label: 'Connect Page',
      description: 'Link Facebook page',
      icon: Facebook,
      color: 'text-blue-600 bg-blue-50 hover:bg-blue-100',
      onClick: () => navigate('/connect-facebook'),
    },
    {
      label: 'View Plans',
      description: 'Upgrade your plan',
      icon: TrendingUp,
      color: 'text-purple-600 bg-purple-50 hover:bg-purple-100',
      onClick: () => navigate('/billing'),
    },
  ];

  return (
    <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-5">
      <h3 className="text-lg font-semibold text-gray-900 mb-4">Quick Actions</h3>
      <div className="grid grid-cols-2 gap-3">
        {actions.map((action) => (
          <button
            key={action.label}
            onClick={action.onClick}
            className={`flex flex-col items-center gap-2 p-4 rounded-lg transition-colors ${action.color}`}
          >
            <action.icon className="w-6 h-6" />
            <span className="text-sm font-medium">{action.label}</span>
            <span className="text-xs opacity-70">{action.description}</span>
          </button>
        ))}
      </div>
    </div>
  );
}
