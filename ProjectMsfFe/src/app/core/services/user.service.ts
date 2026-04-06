import { UserResponse } from '../model/response/user.model';
import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { HttpClient } from '@angular/common/http';
import { PageResult } from '../model/base/api-page-response.model';
import { Observable, map } from 'rxjs';
import { ApiResponse } from '../model/base/api-response.model';
import { UserUpdata } from '../model/update/userupdate.model';
import { UserRequest } from '../model/request/userRequest.model';
import { UserEventsResponse } from '../model/response/user-events-response.model';
import { UserProfile } from '../model/response/userprofile.model';

@Injectable({
  providedIn: 'root',
})
export class UserService extends BaseApiService {
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

  getUserEvents(id?: string | number) {
    const url = id ? `users/events/${id}` : 'users/events';
    return this.get<UserEventsResponse>(url);
  }
  getUserProfile(id: string | number) {
    const url = `users/events/${id}`;
    return this.get<UserProfile>(url);
  }
  GetUserbyid() {
    return this.get<UserResponse>('users');
  }
}
