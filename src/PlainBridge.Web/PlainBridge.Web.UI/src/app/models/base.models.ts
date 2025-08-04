export interface ResultDto<T> {
  data: T;
  code: ResultCodeEnum;
  message: string;
  isSuccess: boolean;
}

export enum ResultCodeEnum {
  Success = 0,
  ValidationError = 1,
  NotFound = 2,
  Unauthorized = 3,
  Forbidden = 4,
  InternalServerError = 5
}

export interface BaseEntity {
  id: number;
  createdAt: Date;
  updatedAt: Date;
}
