import { BaseEntity } from './base.models'; 
 
export interface ServerApplicationDto extends BaseEntity {
  name: string;
  description?: string;
  path: string;
  isActive: boolean;
  userId: number;
  hostApplicationId: number; 
}
