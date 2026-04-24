export class ApiError extends Error {
  constructor(
    public statusCode: number,
    message: string,
    public errors?: { [key: string]: string[] } | null
  ) {
    super(message);
    this.name = 'ApiError';
  }
}