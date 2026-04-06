import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { CreateOrderRequest } from '../model/request/orderRequest.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class OrderService extends BaseApiService {
  constructor(http: HttpClient) {
    super(http);
  }

  createOrder(request: CreateOrderRequest): Observable<any> {
    return this.post<any>('order', request);
  }

  getUserOrders(pageIndex: number = 1, pageSize: number = 10): Observable<any> {
    const params = {
      pageIndex: pageIndex,
      pageSize: pageSize,
    };
    return this.getpage<any>(`order/user`, params);
  }
}
