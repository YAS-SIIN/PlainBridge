import { BaseEntity } from './base.models';

export interface UserDto extends BaseEntity {
  externalId: string;
  username: string;
  email: string;
  phoneNumber?: string;
  name: string;
  family: string;
  userId?: number;
}

export interface ChangeUserPasswordDto {
  id: number;
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface UserProfileDto {
  username: string;
  email: string;
  phoneNumber?: string;
  name: string;
  family: string;
}
