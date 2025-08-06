import { BaseEntity } from './base.models'; 
 
export interface ServerApplicationDto extends BaseEntity {
  appId: string;
  serverApplicationAppId: string; 
  name: string;
  description?: string;
  internalPort: number;
  serverApplicationType: ServerApplicationTypeEnum;
  isActive: boolean;
  userId: number;
  userName: string;
}


export enum ServerApplicationTypeEnum {
  SharePort = 0,
  UsePort = 1
}