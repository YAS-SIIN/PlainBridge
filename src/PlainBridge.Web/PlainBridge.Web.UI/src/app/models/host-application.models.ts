import { BaseEntity } from './base.models';

export interface HostApplicationDto extends BaseEntity {
  appId: string; 
  name: string;
  domain: string;
  internalUrl: string; 
  userId: number; 
  userName: string;
}
