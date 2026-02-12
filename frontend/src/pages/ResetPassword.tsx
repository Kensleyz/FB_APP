import { Zap } from 'lucide-react';
import { Link } from 'react-router-dom';
import { ResetPasswordForm } from '../components/auth/ResetPasswordForm';

export function ResetPassword() {
  return (
    <div className="min-h-screen bg-gray-50 flex flex-col items-center justify-center p-4">
      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <Link to="/" className="inline-flex items-center gap-2 mb-4">
            <Zap className="w-8 h-8 text-primary-600" />
            <span className="text-2xl font-bold text-gray-900">PageBoost AI</span>
          </Link>
          <h1 className="text-2xl font-bold text-gray-900">Reset your password</h1>
          <p className="text-sm text-gray-500 mt-1">Enter your new password below</p>
        </div>

        <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-6">
          <ResetPasswordForm />
        </div>
      </div>
    </div>
  );
}
