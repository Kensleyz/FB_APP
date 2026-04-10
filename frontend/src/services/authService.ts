import api from './api';
import type {
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  UserDto,
  UpdateProfileRequest,
} from '../types/auth';

export const authService = {
  login: (data: LoginRequest) =>
    api.post<{ data: AuthResponse }>('/auth/login', data).then((r) => r.data.data),

  register: (data: RegisterRequest) =>
    api.post<{ data: AuthResponse }>('/auth/register', data).then((r) => r.data.data),

  verifyEmail: (token: string) =>
    api.post('/auth/verify-email', { token }).then((r) => r.data),

  forgotPassword: (data: ForgotPasswordRequest) =>
    api.post('/auth/forgot-password', data).then((r) => r.data),

  resetPassword: (data: ResetPasswordRequest) =>
    api.post('/auth/reset-password', data).then((r) => r.data),

  getMe: () =>
    api.get<{ data: UserDto }>('/auth/me').then((r) => r.data.data),

  updateProfile: (data: UpdateProfileRequest) =>
    api.put<{ data: UserDto }>('/auth/me', data).then((r) => r.data.data),

  refresh: (refreshToken: string) =>
    api.post<{ data: AuthResponse }>('/auth/refresh', { refreshToken }).then((r) => r.data.data),

  logout: () =>
    api.post('/auth/logout').then((r) => r.data),
};
