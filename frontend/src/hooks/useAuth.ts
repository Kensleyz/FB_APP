import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';
import { authService } from '../services/authService';
import type {
  LoginRequest,
  RegisterRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  UpdateProfileRequest,
} from '../types/auth';

export function useAuth() {
  const navigate = useNavigate();
  const { setAuth, setUser, logout: storeLogout } = useAuthStore();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const login = async (data: LoginRequest) => {
    setLoading(true);
    setError(null);
    try {
      const response = await authService.login(data);
      setAuth(response.user, response.accessToken, response.refreshToken);
      navigate('/dashboard');
    } catch (err: unknown) {
      const message =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ||
        'Login failed. Please check your credentials.';
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  const register = async (data: RegisterRequest) => {
    setLoading(true);
    setError(null);
    try {
      const response = await authService.register(data);
      setAuth(response.user, response.accessToken, response.refreshToken);
      navigate('/dashboard');
    } catch (err: unknown) {
      const message =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ||
        'Registration failed. Please try again.';
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  const forgotPassword = async (data: ForgotPasswordRequest) => {
    setLoading(true);
    setError(null);
    try {
      await authService.forgotPassword(data);
      return true;
    } catch (err: unknown) {
      const message =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ||
        'Failed to send reset email.';
      setError(message);
      return false;
    } finally {
      setLoading(false);
    }
  };

  const resetPassword = async (data: ResetPasswordRequest) => {
    setLoading(true);
    setError(null);
    try {
      await authService.resetPassword(data);
      navigate('/login');
      return true;
    } catch (err: unknown) {
      const message =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ||
        'Failed to reset password.';
      setError(message);
      return false;
    } finally {
      setLoading(false);
    }
  };

  const updateProfile = async (data: UpdateProfileRequest) => {
    setLoading(true);
    setError(null);
    try {
      const user = await authService.updateProfile(data);
      setUser(user);
      return true;
    } catch (err: unknown) {
      const message =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ||
        'Failed to update profile.';
      setError(message);
      return false;
    } finally {
      setLoading(false);
    }
  };

  const logout = async () => {
    try {
      await authService.logout();
    } finally {
      storeLogout();
      navigate('/login');
    }
  };

  return {
    login,
    register,
    forgotPassword,
    resetPassword,
    updateProfile,
    logout,
    loading,
    error,
    setError,
  };
}
