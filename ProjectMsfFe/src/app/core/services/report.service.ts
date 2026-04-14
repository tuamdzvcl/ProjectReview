import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
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
    let queryString = `report/revenue?groupBy=${groupBy}`;
    if (fromDate) queryString += `&fromDate=${fromDate}`;
    if (toDate) queryString += `&toDate=${toDate}`;

    return this.get<ReportResponse>(queryString);
  }
}
