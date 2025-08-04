import { BaseEntity } from './base.models';

export interface HostApplicationDto extends BaseEntity {
  name: string;
  description?: string;
  url: string;
  port: number;
  isActive: boolean;
  userId: number;
  serverApplications?: ServerApplicationDto[];
}

export interface ServerApplicationDto extends BaseEntity {
  name: string;
  description?: string;
  path: string;
  isActive: boolean;
  userId: number;
  hostApplicationId: number;
  hostApplication?: HostApplicationDto;
}
