import { Menu, Bell, LogOut, User } from 'lucide-react';
import { useState, useRef, useEffect } from 'react';
import { useAuthStore } from '../../store/authStore';
import { useAuth } from '../../hooks/useAuth';

interface HeaderProps {
  onMenuClick: () => void;
}

export function Header({ onMenuClick }: HeaderProps) {
  const user = useAuthStore((s) => s.user);
  const { logout } = useAuth();
  const [menuOpen, setMenuOpen] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function handleClick(e: MouseEvent) {
      if (menuRef.current && !menuRef.current.contains(e.target as Node)) {
        setMenuOpen(false);
      }
    }
    document.addEventListener('mousedown', handleClick);
    return () => document.removeEventListener('mousedown', handleClick);
  }, []);

  return (
    <header className="sticky top-0 z-30 h-16 bg-white border-b border-gray-200 flex items-center justify-between px-4 lg:px-6">
      <button
        onClick={onMenuClick}
        className="p-2 text-gray-500 hover:text-gray-700 lg:hidden"
      >
        <Menu className="w-5 h-5" />
      </button>

      <div className="flex-1" />

      <div className="flex items-center gap-3">
        <button className="relative p-2 text-gray-500 hover:text-gray-700 rounded-lg hover:bg-gray-100">
          <Bell className="w-5 h-5" />
        </button>

        <div className="relative" ref={menuRef}>
          <button
            onClick={() => setMenuOpen(!menuOpen)}
            className="flex items-center gap-2 p-1.5 rounded-lg hover:bg-gray-100 transition-colors"
          >
            <div className="w-8 h-8 rounded-full bg-primary-100 text-primary-700 flex items-center justify-center text-sm font-semibold">
              {user?.firstName?.[0]}
              {user?.lastName?.[0]}
            </div>
            <span className="text-sm font-medium text-gray-700 hidden sm:block">
              {user?.firstName}
            </span>
          </button>

          {menuOpen && (
            <div className="absolute right-0 mt-2 w-48 bg-white rounded-lg border border-gray-200 shadow-lg py-1 z-50">
              <div className="px-3 py-2 border-b border-gray-100">
                <p className="text-sm font-medium text-gray-900">
                  {user?.firstName} {user?.lastName}
                </p>
                <p className="text-xs text-gray-500">{user?.email}</p>
              </div>
              <a
                href="/settings"
                className="flex items-center gap-2 px-3 py-2 text-sm text-gray-700 hover:bg-gray-50"
              >
                <User className="w-4 h-4" />
                Settings
              </a>
              <button
                onClick={logout}
                className="flex items-center gap-2 w-full px-3 py-2 text-sm text-red-600 hover:bg-red-50"
              >
                <LogOut className="w-4 h-4" />
                Sign Out
              </button>
            </div>
          )}
        </div>
      </div>
    </header>
  );
}
