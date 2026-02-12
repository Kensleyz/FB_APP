import { AlertCircle, CheckCircle2, Info, XCircle } from 'lucide-react';
import type { ReactNode } from 'react';

interface AlertProps {
  type: 'success' | 'error' | 'warning' | 'info';
  children: ReactNode;
  className?: string;
}

const styles = {
  success: {
    container: 'bg-green-50 border-green-200 text-green-800',
    icon: CheckCircle2,
  },
  error: {
    container: 'bg-red-50 border-red-200 text-red-800',
    icon: XCircle,
  },
  warning: {
    container: 'bg-yellow-50 border-yellow-200 text-yellow-800',
    icon: AlertCircle,
  },
  info: {
    container: 'bg-blue-50 border-blue-200 text-blue-800',
    icon: Info,
  },
};

export function Alert({ type, children, className = '' }: AlertProps) {
  const { container, icon: Icon } = styles[type];

  return (
    <div
      className={`flex items-start gap-3 rounded-lg border p-4 ${container} ${className}`}
    >
      <Icon className="w-5 h-5 shrink-0 mt-0.5" />
      <div className="text-sm">{children}</div>
    </div>
  );
}
