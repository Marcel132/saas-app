import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiEndpoints } from '../constants/api-endpoints';
import { ApiResponseModel } from '../models/api-response-model';
import { PentesterReportDto } from '../../features/main/models/pentester-report-dto';

@Injectable({
  providedIn: 'root',
})
export class ReportApi {
  private readonly http = inject(HttpClient);

  getReports(){
    return this.http.get<ApiResponseModel<PentesterReportDto[]>>(
      ApiEndpoints.reports.get
    )
  }
}
