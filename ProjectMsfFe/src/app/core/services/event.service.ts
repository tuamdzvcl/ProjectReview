import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map, of } from 'rxjs';
import { EventModel } from '../model/response/event.model';
import { PageResult } from '../model/base/api-page-response.model';
import { BaseApiService } from './base-api.service';
import { EventRequest } from '../model/request/eventRequest.model';
import { ApiResponse } from '../model/base/api-response.model';

@Injectable({
  providedIn: 'root',
})
export class EventService extends BaseApiService {
  constructor(http: HttpClient) {
    super(http);
  }

  GetEvents(pageIndex: number, pageSize: number, key: string) {
    const params: any = {
      pageIndex: pageIndex,
      pageSize: pageSize,
    };
    if (key) {
      params.key = key;
    }
    return this.getpage<EventModel>('event/page', params).pipe(
      map((res: PageResult<EventModel>) => {
        return {
          items: res.Items,
          pageIndex: res.PageIndex,
          pageSize: res.PageSize,
          totalRecords: res.TotalRecords,
          totalPages: res.TotalPages,
        };
      })
    );
  }

  CreateEvent(data: FormData) {
    return this.post<ApiResponse<EventModel>>('event', data);
  }

  GetEventswithTypeticket(
    pageIndex: number,
    pageSize: number,
    key: string,
    categoryId: number | null = null
  ) {
    const params: any = {
      pageIndex: pageIndex,
      pageSize: pageSize,
    };
    if (key) {
      params.key = key;
    }
    if (categoryId !== null) {
      params.categoryId = categoryId;
    }
    return this.getpage<EventModel>('event/page-with-ticket-types', params).pipe(
      map((res: PageResult<EventModel>) => {
        console.log(params)
        return {
          items: res.Items,
          pageIndex: res.PageIndex,
          pageSize: res.PageSize,
          totalRecords: res.TotalRecords,
          totalPages: res.TotalPages,
        };
      })
    );
  }

  GetEventId(id: string) {
    return this.get<EventModel>(`event/${id}`).pipe(
      map((res: EventModel) => {
        console.log('eventid', res);
        return res;
      })
    );
  }
  UpdateEvent(id: string, data: FormData) {
    return this.put<ApiResponse<EventModel>>(`event/${id}`, data);
  }
  UpdateEventStatus(id: string, status: number) {
    return this.patch<any>(`event/${id}/status`, { Status: status });
  }

  DeleteEvent(id: string) {
    return this.delete<ApiResponse<any>>(`event/${id}`);
  }
}
