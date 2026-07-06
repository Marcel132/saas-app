import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiEndpoints } from '../constants/api-endpoints';
import { ApiResponseModel } from '../models/api-response-model';
import { ReportsDto } from '../../features/main/models/reports-dto';

@Injectable({
  providedIn: 'root',
})
export class ReportApi {
  private readonly http = inject(HttpClient);

  getReports(){
    return this.http.get<ApiResponseModel<ReportsDto[]>>(
      ApiEndpoints.reports.get
    )
  }
}
