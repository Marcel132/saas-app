import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { ApiResponseModel } from '../models/api-response-model';
import { ApiEndpoints } from '../constants/api-endpoints';
import { EditContractDto } from '../../features/main/pages/company-layout/contracts/models/edit-contract-dto';
import { AddContractDto } from '../../features/main/pages/company-layout/contracts/models/add-contract-dto';
import { CompanyApplicationsDto } from '../../features/main/models/company-applications-dto';
import { PagedRequestModel } from '../models/paged-request-model';
import { PagedResponseModel } from '../models/paged-response-model';
import { PublicContractDto } from '../../features/main/models/contracts/public-contract-dto';
import { PentesterContractDto } from '../../features/main/models/contracts/pentester-contract-dto';
import { CompanyContractDto } from '../../features/main/models/contracts/company-contract-dto';
import { ContractDetailsDto } from '../../features/main/models/contracts/contract-details-dto';

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

    return this.http.get<ApiResponseModel<PagedResponseModel<PublicContractDto>>>(
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

    return this.http.get<ApiResponseModel<PagedResponseModel<PentesterContractDto>>>(
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

    return this.http.get<ApiResponseModel<PagedResponseModel<CompanyContractDto>>>(
      `${ApiEndpoints.contracts.contracts}/company`,
      { params }
    )
  }

  getContractDetailsById(contractId: number) {
    return this.http.get<ApiResponseModel<ContractDetailsDto>>(
      `${ApiEndpoints.contracts.contracts}/${contractId}`
    )
  }

  createContract(request: AddContractDto) {
    return this.http.post<ApiResponseModel<null>>(
      ApiEndpoints.contracts.contracts,
      request,
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
