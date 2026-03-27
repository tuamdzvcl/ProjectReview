import { UserResponse } from './../model/user.model';
import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { HttpClient } from '@angular/common/http';
import { PageResult } from '../model/api-page-response.model';
import { Observable, map } from 'rxjs';
import { ApiResponse } from '../model/api-response.model';
import { UserUpdata } from '../model/update/userupdate.model';
import { UserRequest } from '../model/request/userRequest.model';

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
}
