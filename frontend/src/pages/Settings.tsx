import { useState, useEffect } from 'react';
import { useAuthStore } from '../store/authStore';
import { useAuth } from '../hooks/useAuth';
import { useFacebook } from '../hooks/useFacebook';
import { useFacebookStore } from '../store/facebookStore';
import { Input } from '../components/common/Input';
import { Button } from '../components/common/Button';
import { Card, CardHeader } from '../components/common/Card';
import { Alert } from '../components/common/Alert';
import { Facebook, Unlink } from 'lucide-react';
import { formatDate } from '../utils/formatters';

export function Settings() {
  const user = useAuthStore((s) => s.user);
  const { updateProfile, loading, error, setError } = useAuth();
  const { fetchPages, disconnectPage } = useFacebook();
  const pages = useFacebookStore((s) => s.pages);

  const [firstName, setFirstName] = useState(user?.firstName || '');
  const [lastName, setLastName] = useState(user?.lastName || '');
  const [phoneNumber, setPhoneNumber] = useState(user?.phoneNumber || '');
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    fetchPages();
  }, [fetchPages]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(false);
    const result = await updateProfile({ firstName, lastName, phoneNumber: phoneNumber || undefined });
    if (result) setSuccess(true);
  };

  return (
    <div className="space-y-6 max-w-2xl">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Settings</h1>
        <p className="text-sm text-gray-500 mt-1">Manage your profile and preferences.</p>
      </div>

      {/* Profile Form */}
      <Card>
        <CardHeader title="Profile" description="Update your personal information." />

        {error && <Alert type="error" className="mb-4">{error}</Alert>}
        {success && <Alert type="success" className="mb-4">Profile updated successfully.</Alert>}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-3">
            <Input
              label="First Name"
              value={firstName}
              onChange={(e) => setFirstName(e.target.value)}
              required
            />
            <Input
              label="Last Name"
              value={lastName}
              onChange={(e) => setLastName(e.target.value)}
              required
            />
          </div>

          <Input
            label="Email"
            type="email"
            value={user?.email || ''}
            disabled
          />

          <Input
            label="Phone Number"
            type="tel"
            placeholder="+27 XX XXX XXXX"
            value={phoneNumber}
            onChange={(e) => setPhoneNumber(e.target.value)}
          />

          <div className="flex items-center gap-3">
            <Button type="submit" loading={loading}>Save Changes</Button>
            <div className="text-xs text-gray-500">
              Plan: <span className="font-medium text-gray-700">{user?.subscriptionTier}</span>
            </div>
          </div>
        </form>
      </Card>

      {/* Connected Pages */}
      <Card>
        <CardHeader
          title="Connected Facebook Pages"
          description="Manage your linked Facebook Business Pages."
        />

        {pages.length === 0 ? (
          <p className="text-sm text-gray-500 text-center py-4">No pages connected.</p>
        ) : (
          <div className="space-y-3">
            {pages.map((page) => (
              <div
                key={page.id}
                className="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
              >
                <div className="flex items-center gap-3">
                  {page.profilePictureUrl ? (
                    <img
                      src={page.profilePictureUrl}
                      alt={page.pageName}
                      className="w-8 h-8 rounded-full"
                    />
                  ) : (
                    <div className="w-8 h-8 rounded-full bg-blue-100 flex items-center justify-center">
                      <Facebook className="w-4 h-4 text-blue-600" />
                    </div>
                  )}
                  <div>
                    <p className="text-sm font-medium text-gray-900">{page.pageName}</p>
                    <p className="text-xs text-gray-500">
                      Connected {formatDate(page.connectedAt)}
                    </p>
                  </div>
                </div>
                <button
                  onClick={() => disconnectPage(page.id)}
                  className="p-1.5 text-gray-400 hover:text-red-600 rounded-lg hover:bg-red-50"
                  title="Disconnect page"
                >
                  <Unlink className="w-4 h-4" />
                </button>
              </div>
            ))}
          </div>
        )}
      </Card>
    </div>
  );
}
