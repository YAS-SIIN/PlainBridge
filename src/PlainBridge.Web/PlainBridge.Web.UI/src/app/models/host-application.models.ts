import { BaseEntity } from './base.models';

export interface HostApplicationDto extends BaseEntity {
  appId: string; 
  name: string;
  domain: string;
  description?: string;
  internalUrl: string; 
  isActive: boolean;
  userId: number; 
}
 