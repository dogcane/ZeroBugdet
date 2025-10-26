export interface User {
  id: string;
  email: string;
  isMainUser: boolean;
  createdAt?: Date;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  message: string;
  token: string;
  userId: string;
  email: string;
  roles: string[];
  isMainUser: boolean;
}

export interface RegisterMainUserRequest {
  email: string;
  password: string;
  confirmPassword: string;
}

export interface InviteUserRequest {
  email: string;
}

export interface UserInvitation {
  id: number;
  email: string;
  token: string;
  isUsed: boolean;
  expiresAt: Date;
  createdAt: Date;
}

export interface CompleteRegistrationRequest {
  token: string;
  password: string;
  confirmPassword: string;
}
