import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, catchError, throwError, Observable } from 'rxjs';
import { ApiResponse } from '../model/base/api-response.model';
import { ApiError } from '../model/base/ApiError.model';
import { PageResult } from '../model/base/api-page-response.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class BaseApiService {
  protected baseUrl = environment.apiBaseUrl;

  constructor(protected http: HttpClient) { }

  private handleError(error: HttpErrorResponse) {
    if (error.error && error.error.StatusCode) {
      return throwError(() => new ApiError(error.error.StatusCode, error.error.Message || 'Có lỗi xảy ra', error.error.Errors));
    }
    return throwError(() => error);
  }

  get<T>(url: string): Observable<T> {
    return this.http.get<ApiResponse<T>>(`${this.baseUrl}/${url}`).pipe(
      map((res) => {
        if (!res.Success) {
          throw new ApiError(res.StatusCode, res.Message, res.Errors);
        }
        return res.Data;
      }),
      catchError((err) => this.handleError(err))
    );
  }

  post<T>(url: string, body: any): Observable<T> {
    return this.http.post<ApiResponse<T>>(`${this.baseUrl}/${url}`, body).pipe(
      map((res) => {
        if (!res.Success) {
          throw new ApiError(res.StatusCode, res.Message, res.Errors);
        }
        return res.Data;
      }),
      catchError((err) => this.handleError(err))
    );
  }

  getpage<T>(url: string, params?: any): Observable<PageResult<T>> {
    return this.http
      .get<PageResult<T>>(`${this.baseUrl}/${url}`, {
        params: params,
      })
      .pipe(
        map((res) => {
          if (!res.Success) {
            throw new ApiError(res.StatusCode, res.Message);
          }
          return res;
        }),
        catchError((err) => this.handleError(err))
      );
  }

  getById<T>(url: string, id: string | number, params?: any): Observable<T> {
    return this.get<T>(`${url}/${id}`);
  }

  put<T>(url: string, body: any): Observable<T> {
    return this.http.put<ApiResponse<T>>(`${this.baseUrl}/${url}`, body).pipe(
      map((res) => {
        if (!res.Success) {
          throw new ApiError(res.StatusCode, res.Message, res.Errors);
        }
        return res.Data;
      }),
      catchError((err) => this.handleError(err))
    );
  }

  putById<T>(url: string, id: string | number, body: any): Observable<T> {
    return this.put<T>(`${url}/${id}`, body);
  }

  patch<T>(url: string, body: any): Observable<T> {
    return this.http.patch<ApiResponse<T>>(`${this.baseUrl}/${url}`, body).pipe(
      map((res) => {
        if (!res.Success) {
          throw new ApiError(res.StatusCode, res.Message, res.Errors);
        }
        return res.Data;
      }),
      catchError((err) => this.handleError(err))
    );
  }

  delete<T>(url: string, params?: any): Observable<T> {
    return this.http
      .delete<ApiResponse<T>>(`${this.baseUrl}/${url}`, { params })
      .pipe(
        map((res) => {
          if (!res.Success) {
            throw new ApiError(res.StatusCode, res.Message, res.Errors);
          }
          return res.Data;
        }),
        catchError((err) => this.handleError(err))
      );
  }

  deleteById<T>(url: string, id: string | number): Observable<T> {
    return this.delete<T>(`${url}/${id}`);
  }
}
