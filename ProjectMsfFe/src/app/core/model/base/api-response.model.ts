export interface ApiResponse<T> {
  StatusCode: number;
  Success: boolean;
  Message: string;
  Data: T;
  Timestamp: string;
  Errors?: { [key: string]: string[] } | null;
}