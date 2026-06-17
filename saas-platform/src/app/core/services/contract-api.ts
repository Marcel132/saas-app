import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { ApiResponseModel } from '../models/api-response-model';
import { ApiEndpoints } from '../constants/api-endpoints';
import { OffersResponseDto } from '../../features/main/models/offers-response-dto';
import { ContractDto } from '../../features/main/models/contract-dto';
import { EditContractDto } from '../../features/main/pages/company-layout/contracts/models/edit-contract-dto';
import { AddContractDto } from '../../features/main/pages/company-layout/contracts/models/add-contract-dto';
import { CompanyApplicationsDto } from '../../features/main/models/company-applications-dto';
import { PagedRequestModel } from '../models/paged-request-model';
import { PagedResponseModel } from '../models/paged-response-model';

@Injectable({
  providedIn: 'root',
})
export class ContractApi {

  private readonly http = inject(HttpClient)

  getPublicContracts(paramsReq: PagedRequestModel) {
    let params = new HttpParams()
      .set('page', paramsReq.page)
      .set('pageSize', paramsReq.pageSize)

    if (paramsReq.search?.trim()) {
      params.set('search', paramsReq.search)
    }

    return this.http.get<ApiResponseModel<PagedResponseModel<ContractDto>>>(
      `${ApiEndpoints.contracts.contracts}/public`,
      { params }
    )
  }
  getPentesterContracts(paramsReq: PagedRequestModel) {
    let params = new HttpParams()
      .set('page', paramsReq.page)
      .set('pageSize', paramsReq.pageSize)

    if (paramsReq.search?.trim()) {
      params.set('search', paramsReq.search)
    }

    return this.http.get<ApiResponseModel<PagedResponseModel<ContractDto>>>(
      `${ApiEndpoints.contracts.contracts}`,
      { params }
    )
  }
  getCompanyContracts(paramsReq: PagedRequestModel) {
    let params = new HttpParams()
      .set('page', paramsReq.page)
      .set('pageSize', paramsReq.pageSize)

    if (paramsReq.search?.trim()) {
      params.set('search', paramsReq.search)
    }

    return this.http.get<ApiResponseModel<PagedResponseModel<ContractDto>>>(
      `${ApiEndpoints.contracts.contracts}/company`,
      { params }
    )
  }

  getContractDetailsById(contractId: number){
    return this.http.get<ApiResponseModel<ContractDto>>(
      `${ApiEndpoints.contracts.contracts}/${contractId}`
    )
  }

  createContract(request: AddContractDto) {
    return this.http.post<ApiResponseModel<null>>(
      ApiEndpoints.contracts.contracts,
      request,
    )
  }

  //   return this.http.get<ApiResponseModel<PagedResponseModel<ContractDto>>>(
  //     `${ApiEndpoints.contracts.contracts}`,
  //     { params }
  //   )
  // }
  getContractById(contractId: number) {
    return this.http.get<ApiResponseModel<ContractDto>>(
      `${ApiEndpoints.contracts.contracts}/${contractId}`,
    )
  }
  updateContract(contractId: number, request: EditContractDto) {
    return this.http.patch<ApiResponseModel<null>>(
      `${ApiEndpoints.contracts.contracts}/${contractId}`,
      request,
    )
  }
  deleteContractSoft(contractId: number) {
    return this.http.patch<ApiResponseModel<null>>(
      `${ApiEndpoints.contracts.contracts}/${contractId}/close`,
      {},
    )
  }
  createApplication(contractId: number) {
    return this.http.post<ApiResponseModel<null>>(
      `${ApiEndpoints.contracts.contracts}/${contractId}/applications`,
      {},
    )
  }
  getContractApplications(contractId: number) {
    return this.http.get<ApiResponseModel<CompanyApplicationsDto[]>>(
      `${ApiEndpoints.contracts.contracts}/${contractId}/applications`,
    )
  }
}
