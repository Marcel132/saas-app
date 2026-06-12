import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiResponseModel } from '../models/api-response-model';
import { ApiEndpoints } from '../constants/api-endpoints';

@Injectable({
  providedIn: 'root',
})
export class ApplicationApi {
  private readonly http = inject(HttpClient)

  acceptApplication(id: number){
    return this.http.patch<ApiResponseModel<object>>(
      `${ApiEndpoints.applications.applications}/${id}/accept`,
      {},
      {withCredentials: true}
    )
  }

  rejectApplication(id: number){
    return this.http.patch<ApiResponseModel<object>>(
      `${ApiEndpoints.applications.applications}/${id}/reject`,
      {},
      {withCredentials: true}
    )
  }
}
