import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { BaseApiService } from './base-api.service';
import { ReportResponse } from '../model/response/report.model';

@Injectable({
  providedIn: 'root',
})
export class ReportService extends BaseApiService {
  constructor(http: HttpClient) {
    super(http);
  }

  GetRevenueReport(fromDate?: string, toDate?: string, groupBy: string = 'monthly'): Observable<ReportResponse> {
    const params: any = { groupBy };
    if (fromDate) params.fromDate = fromDate;
    if (toDate) params.toDate = toDate;

    return this.http.get<any>(`${this.baseUrl}/report/revenue`, { params }).pipe(
      map((res: any) => {
        return res.Data;
      })
    );
  }
}
