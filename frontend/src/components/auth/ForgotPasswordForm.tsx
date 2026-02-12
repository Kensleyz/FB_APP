import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Button } from '../common/Button';
import { Input } from '../common/Input';
import { Alert } from '../common/Alert';
import { useAuth } from '../../hooks/useAuth';
import { isValidEmail } from '../../utils/validators';

export function ForgotPasswordForm() {
  const { forgotPassword, loading, error, setError } = useAuth();
  const [email, setEmail] = useState('');
  const [sent, setSent] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!isValidEmail(email)) {
      setError('Please enter a valid email address.');
      return;
    }

    const success = await forgotPassword({ email });
    if (success) setSent(true);
  };

  if (sent) {
    return (
      <div className="text-center space-y-4">
        <Alert type="success">
          We've sent a password reset link to <strong>{email}</strong>. Please check your inbox.
        </Alert>
        <Link
          to="/login"
          className="text-sm text-primary-600 hover:text-primary-700 font-medium"
        >
          Back to sign in
        </Link>
      </div>
    );
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {error && <Alert type="error">{error}</Alert>}

      <p className="text-sm text-gray-600">
        Enter your email address and we'll send you a link to reset your password.
      </p>

      <Input
        label="Email"
        type="email"
        placeholder="you@example.com"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        autoComplete="email"
        required
      />

      <Button type="submit" loading={loading} className="w-full">
        Send Reset Link
      </Button>

      <p className="text-center text-sm text-gray-600">
        <Link to="/login" className="text-primary-600 hover:text-primary-700 font-medium">
          Back to sign in
        </Link>
      </p>
    </form>
  );
}
