
export interface PageResult<T> {
  Items: T[];
  TotalRecords: number;
  PageIndex: number;
  PageSize: number;
  TotalPages:number
  StatusCode: number;
  Success: boolean;
  Message: string;
  Timestamp: string;

}