import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BaseApiService } from './base-api.service';
import { PromotionRequest, PromotionResponse } from '../model/response/promotion.model';
import { Observable } from 'rxjs';
import { PageResult } from '../model/base/api-page-response.model';

@Injectable({
  providedIn: 'root'
})
export class PromotionService extends BaseApiService {
  constructor(http: HttpClient) {
    super(http);
  }

  getAll(params: any): Observable<PageResult<PromotionResponse>> {
    return this.getpage<PromotionResponse>('promotion/all', params);
  }

  create(data: PromotionRequest): Observable<any> {
    return this.post<any>('promotion/admin', data);
  }

  update(id: number, data: PromotionRequest): Observable<any> {
    return this.putById<any>('promotion/admin', id, data);
  }

  getByUserId(id: number): Observable<PromotionResponse> {
    return this.get<PromotionResponse>(`promotion/admin/${id}`);
  }

  deletePromotion(id: number): Observable<any> {
    return this.deleteById('promotion/admin', id);
  }

  importExcel(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post(`${this.baseUrl}/promotion/admin/import`, formData);
  }

  exportExcel(): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/promotion/admin/export`, {
      responseType: 'blob',
    });
  }

  downloadTemplate(): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/promotion/admin/template`, {
      responseType: 'blob',
    });
  }
}
