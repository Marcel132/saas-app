import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiResponseModel } from '../models/api-response-model';
import { ApplicationDto } from '../../features/main/models/application-dto';
import { ApiEndpoints } from '../constants/api-endpoints';

@Injectable({
  providedIn: 'root',
})
export class MeApi {
  private readonly http = inject(HttpClient)

  getApplications(){
    return this.http.get<ApiResponseModel<ApplicationDto[]>>(
      ApiEndpoints.me.applications,
      {withCredentials: true}
    )
  }
}
