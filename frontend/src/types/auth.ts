export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  token: string;
  newPassword: string;
  confirmPassword: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  user: UserDto;
}

export interface UserDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  subscriptionTier: SubscriptionTier;
  subscriptionExpiresAt?: string;
  isEmailVerified: boolean;
  createdAt: string;
  lastLoginAt?: string;
}

export type SubscriptionTier = 'Free' | 'Starter' | 'Growth' | 'Pro';

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  phoneNumber?: string;
}
