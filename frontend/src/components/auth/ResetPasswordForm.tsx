import { useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { Button } from '../common/Button';
import { Input } from '../common/Input';
import { Alert } from '../common/Alert';
import { useAuth } from '../../hooks/useAuth';
import { isValidPassword, passwordsMatch } from '../../utils/validators';

export function ResetPasswordForm() {
  const { token } = useParams<{ token: string }>();
  const { resetPassword, loading, error, setError } = useAuth();
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    const pwError = isValidPassword(password);
    if (pwError) {
      setError(pwError);
      return;
    }
    if (!passwordsMatch(password, confirmPassword)) {
      setError('Passwords do not match.');
      return;
    }
    if (!token) {
      setError('Invalid reset link.');
      return;
    }

    resetPassword({ token, newPassword: password, confirmPassword });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {error && <Alert type="error">{error}</Alert>}

      <p className="text-sm text-gray-600">Enter your new password below.</p>

      <Input
        label="New Password"
        type="password"
        placeholder="Min. 8 characters"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        autoComplete="new-password"
        required
      />

      <Input
        label="Confirm Password"
        type="password"
        placeholder="Confirm your password"
        value={confirmPassword}
        onChange={(e) => setConfirmPassword(e.target.value)}
        autoComplete="new-password"
        required
      />

      <Button type="submit" loading={loading} className="w-full">
        Reset Password
      </Button>

      <p className="text-center text-sm text-gray-600">
        <Link to="/login" className="text-primary-600 hover:text-primary-700 font-medium">
          Back to sign in
        </Link>
      </p>
    </form>
  );
}
