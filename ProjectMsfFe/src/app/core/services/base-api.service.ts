import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiResponse } from '../model/api-response.model';
import { ApiError } from '../model/ApiError.model';
import { PageResult } from '../model/api-page-response.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class BaseApiService {

  protected baseUrl = environment.apiBaseUrl;

  constructor(protected http: HttpClient) {}

  get<T>(url:string):Observable<T>{
    return this.http.get<ApiResponse<T>>(
      `${this.baseUrl}/${url}`
    ).pipe(map(res =>{
      if (!res.Success) {
              throw new ApiError(res.StatusCode, res.Message);
        }
        return res.Data;
    }))
  }

  post<T>(url: string, body: any): Observable<T> {
    return this.http.post<ApiResponse<T>>(
      `${this.baseUrl}/${url}`,
      body
    ).pipe(
      map(res => {
        if (!res.Success) {
              throw new ApiError(res.StatusCode, res.Message);
        }
        return res.Data;
      })
    );
  }

  getpage<T>(url: string, params?: any): Observable<PageResult<T>> {
    
  return this.http.get<PageResult<T>>(
    `${this.baseUrl}/${url}`,
    {
      params: params
    }
  ).pipe(
    map(res => {
      if (!res.Success) {
        throw new ApiError(res.StatusCode, res.Message);
      }
      return res;
    })
  );
}

  getById<T>(url: string, id: string | number, params?: any): Observable<T> {
    return this.get<T>(`${url}/${id}`);
  }

  put<T>(url: string, body: any): Observable<T> {
    return this.http.put<ApiResponse<T>>(
      `${this.baseUrl}/${url}`,
      body
    ).pipe(
      map(res => {
        if (!res.Success) {
          throw new ApiError(res.StatusCode, res.Message);
        }
        return res.Data;
      })
    );
  }

  putById<T>(url: string, id: string | number, body: any): Observable<T> {
    return this.put<T>(`${url}/${id}`, body);
  }

  patch<T>(url: string, body: any): Observable<T> {
    return this.http.patch<ApiResponse<T>>(
      `${this.baseUrl}/${url}`,
      body
    ).pipe(
      map(res => {
        if (!res.Success) {
          throw new ApiError(res.StatusCode, res.Message);
        }
        return res.Data;
      })
    );
  }

  delete<T>(url: string, params?: any): Observable<T> {
    return this.http.delete<ApiResponse<T>>(
      `${this.baseUrl}/${url}`,
      { params }
    ).pipe(
      map(res => {
        if (!res.Success) {
          throw new ApiError(res.StatusCode, res.Message);
        }
        return res.Data;
      })
    );
  }

  deleteById<T>(url: string, id: string | number): Observable<T> {
    return this.delete<T>(`${url}/${id}`);
  }
}