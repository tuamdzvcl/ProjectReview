import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api.service';
import { ApiResponse } from '../model/api-response.model';
import { CatetoryResponse } from '../model/catetory.model';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CatetoryService extends BaseApiService {

  constructor(http: HttpClient) {
    super(http);
  }

  GetCatetory() {
    return this.get('catetory').pipe(
      map((res: any) => {
        console.log('Category API Response:', res);
        return {
          Data: res
        }
      }))
  }
}
