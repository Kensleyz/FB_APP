import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Button } from '../common/Button';
import { Input } from '../common/Input';
import { Alert } from '../common/Alert';
import { useAuth } from '../../hooks/useAuth';
import { isValidEmail, isValidPassword, passwordsMatch } from '../../utils/validators';

export function RegisterForm() {
  const { register, loading, error, setError } = useAuth();
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!firstName.trim() || !lastName.trim()) {
      setError('Please enter your full name.');
      return;
    }
    if (!isValidEmail(email)) {
      setError('Please enter a valid email address.');
      return;
    }
    const pwError = isValidPassword(password);
    if (pwError) {
      setError(pwError);
      return;
    }
    if (!passwordsMatch(password, confirmPassword)) {
      setError('Passwords do not match.');
      return;
    }

    register({ firstName, lastName, email, password, confirmPassword });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {error && <Alert type="error">{error}</Alert>}

      <div className="grid grid-cols-2 gap-3">
        <Input
          label="First Name"
          placeholder="Thabo"
          value={firstName}
          onChange={(e) => setFirstName(e.target.value)}
          required
        />
        <Input
          label="Last Name"
          placeholder="Molefe"
          value={lastName}
          onChange={(e) => setLastName(e.target.value)}
          required
        />
      </div>

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
        Create Account
      </Button>

      <p className="text-center text-sm text-gray-600">
        Already have an account?{' '}
        <Link to="/login" className="text-primary-600 hover:text-primary-700 font-medium">
          Sign in
        </Link>
      </p>
    </form>
  );
}
