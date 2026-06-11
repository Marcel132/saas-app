import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { ApiResponseModel } from '../models/api-response-model';
import { ApiEndpoints } from '../constants/api-endpoints';
import { OffersResponseDto } from '../../features/main/models/offers-response-dto';
import { ContractDto } from '../../features/main/models/contract-dto';
import { EditContractDto } from '../../features/main/pages/company-layout/contracts/models/edit-contract-dto';
import { AddContractDto } from '../../features/main/pages/company-layout/contracts/models/add-contract-dto';

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
    return this.http.get<ApiResponseModel<ContractDto>>(
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

  updateContract(id: number, request: EditContractDto){
    return this.http.patch<ApiResponseModel<object>>(
      `${ApiEndpoints.contracts.contracts}/${id}`,
      request,
      {withCredentials: true}
    )
  }
  craeteContract(request: AddContractDto){
    return this.http.post<ApiResponseModel<object>>(
      ApiEndpoints.contracts.contracts,
      request,
      {withCredentials: true}
    )
  }
}
