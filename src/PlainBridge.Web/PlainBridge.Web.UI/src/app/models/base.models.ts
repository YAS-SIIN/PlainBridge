export interface ResultDto<T> {
  data: T;
  errors: { [key: string]: string[] } | null;
  resultCode: ResultCodeEnum;
  resultDescription: string;
  errorDetail: string;
}

export enum ResultCodeEnum {
  Success = 0,
  NotFound = 100,
  Error = 101,
  RepeatedData = 102,
  InvalidData = 103,
  NullData = 104,
  NotDelete = 105,
}

export interface BaseEntity {
  id: number;
  description?: string;
  isActive: RowStateEnum;
  createdAt: Date;
  updatedAt: Date;
}

export enum RowStateEnum {
    Active = 1,
    Inactive = 2,
}
