import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiResponseModel } from '../models/api-response-model';
import { UserApplicationDto } from '../../features/main/models/application-dto';
import { ApiEndpoints } from '../constants/api-endpoints';
import { OffersResponseDto } from '../../features/main/models/offers-response-dto';

@Injectable({
  providedIn: 'root',
})
export class MeApi {
  private readonly http = inject(HttpClient)

  getApplications(){
    return this.http.get<ApiResponseModel<UserApplicationDto[]>>(
      ApiEndpoints.me.applications,
      {withCredentials: true}
    )
  }

  getContracts(){
    return this.http.get<ApiResponseModel<OffersResponseDto>>(
      ApiEndpoints.me.contracts,
      {withCredentials: true}
    )
  }
}
