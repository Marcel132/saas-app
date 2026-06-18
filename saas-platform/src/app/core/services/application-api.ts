import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiResponseModel } from '../models/api-response-model';
import { ApiEndpoints } from '../constants/api-endpoints';
import { UserApplicationDto } from '../../features/main/models/application-dto';

@Injectable({
  providedIn: 'root',
})
export class ApplicationApi {
  private readonly http = inject(HttpClient)

  acceptApplication(applicationId: number){
    return this.http.patch<ApiResponseModel<null>>(
      `${ApiEndpoints.applications.applications}/${applicationId}/accept`,
      {},
    )
  }

  rejectApplication(applicationId: number){
    return this.http.patch<ApiResponseModel<null>>(
      `${ApiEndpoints.applications.applications}/${applicationId}/reject`,
      {},
    )
  }

  getApplications(){
    return this.http.get<ApiResponseModel<UserApplicationDto[]>>(
      `${ApiEndpoints.applications.applications}/me/applications`
    )
  }
}
