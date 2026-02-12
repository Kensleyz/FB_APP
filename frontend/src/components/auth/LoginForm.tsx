import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Button } from '../common/Button';
import { Input } from '../common/Input';
import { Alert } from '../common/Alert';
import { useAuth } from '../../hooks/useAuth';
import { isValidEmail } from '../../utils/validators';

export function LoginForm() {
  const { login, loading, error, setError } = useAuth();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!isValidEmail(email)) {
      setError('Please enter a valid email address.');
      return;
    }
    if (!password) {
      setError('Please enter your password.');
      return;
    }

    login({ email, password });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {error && <Alert type="error">{error}</Alert>}

      <Input
        label="Email"
        type="email"
        placeholder="you@example.com"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        autoComplete="email"
        required
      />

      <Input
        label="Password"
        type="password"
        placeholder="Enter your password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        autoComplete="current-password"
        required
      />

      <div className="flex items-center justify-between">
        <label className="flex items-center gap-2">
          <input type="checkbox" className="rounded border-gray-300 text-primary-600" />
          <span className="text-sm text-gray-600">Remember me</span>
        </label>
        <Link
          to="/forgot-password"
          className="text-sm text-primary-600 hover:text-primary-700"
        >
          Forgot password?
        </Link>
      </div>

      <Button type="submit" loading={loading} className="w-full">
        Sign In
      </Button>

      <p className="text-center text-sm text-gray-600">
        Don't have an account?{' '}
        <Link to="/register" className="text-primary-600 hover:text-primary-700 font-medium">
          Sign up free
        </Link>
      </p>
    </form>
  );
}
