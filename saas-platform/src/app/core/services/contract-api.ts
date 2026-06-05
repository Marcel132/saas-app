import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { ApiResponseModel } from '../models/api-response-model';
import { ApiEndpoints } from '../constants/api-endpoints';
import { OffersResponseDto } from '../../features/main/models/offers-response-dto';
import { OffersDto } from '../../features/main/models/offers-dto';

@Injectable({
  providedIn: 'root',
})
export class ContractApi {

  private readonly http = inject(HttpClient)

  getContracts(){
    return this.http.get<ApiResponseModel<OffersResponseDto>>(
      ApiEndpoints.contracts.contracts,
      {withCredentials: true}
    )
  }

  getContractById(id: number){
    return this.http.get<ApiResponseModel<OffersDto>>(
      `${ApiEndpoints.contracts.contracts}/${id}`,
      {withCredentials: true}
    )
  }

  createApplication(id: number){
    console.log("Creating...")
    return this.http.post<ApiResponseModel<object>>(
      `${ApiEndpoints.contracts.contracts}/${id}/applications`,
      {},
      {
        withCredentials: true,
      }
    )
  }
}
