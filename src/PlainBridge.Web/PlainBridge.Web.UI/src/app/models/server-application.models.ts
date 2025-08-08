import { BaseEntity } from './base.models'; 
 
export interface ServerApplicationDto extends BaseEntity {
  appId: string;
  serverApplicationAppId: string; 
  name: string; 
  internalPort: number;
  serverApplicationType: ServerApplicationTypeEnum; 
  userId: number;
  userName: string;
}


export enum ServerApplicationTypeEnum {
  SharePort = 0,
  UsePort = 1
}