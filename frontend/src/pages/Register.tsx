import { Zap } from 'lucide-react';
import { Link } from 'react-router-dom';
import { RegisterForm } from '../components/auth/RegisterForm';

export function Register() {
  return (
    <div className="min-h-screen bg-gray-50 flex flex-col items-center justify-center p-4">
      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <Link to="/" className="inline-flex items-center gap-2 mb-4">
            <Zap className="w-8 h-8 text-primary-600" />
            <span className="text-2xl font-bold text-gray-900">PageBoost AI</span>
          </Link>
          <h1 className="text-2xl font-bold text-gray-900">Create your account</h1>
          <p className="text-sm text-gray-500 mt-1">Start boosting your Facebook page for free</p>
        </div>

        <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-6">
          <RegisterForm />
        </div>
      </div>
    </div>
  );
}
