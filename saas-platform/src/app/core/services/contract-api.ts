import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { ApiResponseModel } from '../models/api-response-model';
import { ApiEndpoints } from '../constants/api-endpoints';
import { OffersResponseDto } from '../../features/main/models/offers-response-dto';
import { ContractDto } from '../../features/main/models/contract-dto';
import { EditContractDto } from '../../features/main/pages/company-layout/contracts/models/edit-contract-dto';
import { AddContractDto } from '../../features/main/pages/company-layout/contracts/models/add-contract-dto';
import { CompanyApplicationsDto } from '../../features/main/models/company-applications-dto';

@Injectable({
  providedIn: 'root',
})
export class ContractApi {

  private readonly http = inject(HttpClient)

  createContract(request: AddContractDto){
    return this.http.post<ApiResponseModel<null>>(
      ApiEndpoints.contracts.contracts,
      request,
    )
  }
  getAllContracts(){
    return this.http.get<ApiResponseModel<OffersResponseDto>>(
      ApiEndpoints.contracts.contracts,
    )
  }
  getContractById(contractId: number){
    return this.http.get<ApiResponseModel<ContractDto>>(
      `${ApiEndpoints.contracts.contracts}/${contractId}`,
    )
  }
  updateContract(contractId: number, request: EditContractDto){
    return this.http.patch<ApiResponseModel<null>>(
      `${ApiEndpoints.contracts.contracts}/${contractId}`,
      request,
    )
  }
  deleteContractSoft(contractId: number){
    return this.http.patch<ApiResponseModel<null>>(
      `${ApiEndpoints.contracts.contracts}/${contractId}/close`,
      {},
    )
  }
  createApplication(contractId: number){
    return this.http.post<ApiResponseModel<null>>(
      `${ApiEndpoints.contracts.contracts}/${contractId}/applications`,
      {},
    )
  }
  getContractApplications(contractId: number){
    return this.http.get<ApiResponseModel<CompanyApplicationsDto[]>>(
      `${ApiEndpoints.contracts.contracts}/${contractId}/applications`,
    )
  }
}
