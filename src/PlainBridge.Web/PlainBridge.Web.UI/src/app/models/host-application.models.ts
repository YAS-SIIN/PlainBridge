import { BaseEntity } from './base.models';

export interface HostApplicationDto extends BaseEntity {
  name: string;
  description?: string;
  url: string;
  port: number;
  isActive: boolean;
  userId: number; 
}
 