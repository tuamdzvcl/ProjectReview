import { UserResponse } from '../model/response/user.model';
import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { HttpClient } from '@angular/common/http';
import { PageResult } from '../model/base/api-page-response.model';
import { Observable, map, of, shareReplay, tap } from 'rxjs';
import { ApiResponse } from '../model/base/api-response.model';
import { UserUpdata } from '../model/update/userupdate.model';
import { UserRequest } from '../model/request/userRequest.model';
import { UserEventsResponse } from '../model/response/user-events-response.model';
import { UserProfile } from '../model/response/userprofile.model';
import { UserInEvent, ParticipantSummary } from '../model/response/participant.model';

@Injectable({
  providedIn: 'root',
})
export class UserService extends BaseApiService {
  private currentUser$: Observable<UserResponse> | null = null;

  constructor(http: HttpClient) {
    super(http);
  }
  GetUsers(pageIndex: number, pageSize: number, Keyword: string, role: string) {
    const params: any = {
      pageIndex: pageIndex,
      pageSize: pageSize,
    };
    if (Keyword) {
      params.key = Keyword;
    }
    if (role) {
      params.role = role;
    }
    return this.getpage<UserResponse>('users/page/user', params).pipe(
      map((res: PageResult<UserResponse>) => {
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

  deleteUser(id: string | number) {
    return this.deleteById<any>('users', id);
  }

  UpdateUser(id: string | number, data: UserUpdata) {
    return this.putById<ApiResponse<UserResponse>>('users', id, data);
  }
  CreateUser(data: UserRequest) {
    return this.post<ApiResponse<UserResponse>>('users', data)
  }

  getUserEvents() {
    const url = 'users/events';
    return this.get<UserEventsResponse>(url);
  }
  getUserProfile(id: string | number) {
    const url = `users/events/${id}`;
    return this.get<UserProfile>(url);
  }
  GetUserbyid() {
    if (!this.currentUser$) {
      this.currentUser$ = this.get<UserResponse>('users/me').pipe(
        shareReplay(1)
      );
    }
    return this.currentUser$;
  }

  clearUserCache() {
    this.currentUser$ = null;
  }

  GetParticipants(pageIndex: number, pageSize: number): Observable<{
    items: ParticipantSummary[];
    pageIndex: number;
    pageSize: number;
    totalRecords: number;
    totalPages: number;
  }> {
    const params: any = {
      PageIndex: pageIndex,
      PageSize: pageSize,
    };
    return this.getpage<ParticipantSummary>('users/participants/summary', params).pipe(
      map((res: PageResult<ParticipantSummary>) => {
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

  GetParticipantDetail(userId: string): Observable<UserInEvent> {
    return this.get<UserInEvent>(`users/participants/${userId}/detail`);
  }

  updateAvatar(file: File): Observable<ApiResponse<string>> {
    const formData = new FormData();
    formData.append('file', file);
    return this.put<ApiResponse<string>>('users/avatar', formData);
  }

  // 1205/2026-thay đổi
  GetParticipantsByEvent(eventId: string, pageIndex: number, pageSize: number): Observable<{
    items: UserInEvent[];
    pageIndex: number;
    pageSize: number;
    totalRecords: number;
    totalPages: number;
  }> {
    const params: any = {
      PageIndex: pageIndex,
      PageSize: pageSize,
    };
    return this.getpage<UserInEvent>(`users/events/${eventId}/participants`, params).pipe(
      map((res: PageResult<UserInEvent>) => {
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
}
