import { inject, Injectable, signal } from "@angular/core";
import { catchError, EMPTY, switchMap, tap, throwError } from "rxjs";

import { ContractDto } from "../models/contract-dto";
import { MeApi } from "../../../core/services/me-api";
import { ContractApi } from "../../../core/services/contract-api";
import { EditContractDto } from "../pages/company-layout/contracts/models/edit-contract-dto";
import { AddContractDto } from "../pages/company-layout/contracts/models/add-contract-dto";
import { CompanyApplicationsDto } from "../models/company-applications-dto";
import { ApplicationApi } from "../../../core/services/application-api";
import { RequestState } from "../../../core/models/request-state";
import { CONTRACT_STATUS_ORDER } from "../../auth/constants/contract-status-order";
import { APPLICATIONS_STATUS_ORDER } from "../../auth/constants/application-status-order";
import { PagedRequestModel } from "../../../core/models/paged-request-model";
import { PagedResponseModel } from "../../../core/models/paged-response-model";

@Injectable({
  providedIn: 'root'
})

export class CompanyStore {
  // DI
  private readonly meApi = inject(MeApi);
  private readonly contractApi = inject(ContractApi);
  private readonly applicationApi = inject(ApplicationApi);

  // SIGNALS
  request = signal<RequestState>({
    state: 'idle',
    message: ''
  })

  readonly contracts = signal<ContractDto[]>([])
  readonly selectedContract = signal<ContractDto | null>(null)

  readonly applications = signal<CompanyApplicationsDto[]>([])

  // CONST
  private readonly pageSize = 4;

  readonly pagedRequest = signal<PagedRequestModel>({
    page: 1,
    pageSize: this.pageSize,
    search: null
  })

  readonly pagedResponse = signal<PagedResponseModel<ContractDto> | null >(null)

  getContracts() {
    this.request.set({
      state: 'loading',
      message: "Ładowanie kontraktów..."
    })

    return this.contractApi.getCompanyContracts(this.pagedRequest())
      .pipe(
        tap(res => {
          if (res.data) {
            res.data.items.sort(
              (a, b) => CONTRACT_STATUS_ORDER[a.contractStatus] - CONTRACT_STATUS_ORDER[b.contractStatus]
            )
            this.pagedResponse.set(res.data);
            this.contracts.set(res.data.items)
            this.request.set({
              state: 'success',
              message: 'Pobrano Kontrakty'
            })
          }
        }),
        catchError(error => {
          this.request.set({
            state: 'error',
            message: error.error.message
          })

          return throwError(() => error)
        })
      )
  }

  loadOffers(force: boolean = false, page: number | null = null){
      if(!force && this.contracts().length > 0){
        console.log("empty")
        return EMPTY;
      }

      if(page != null && page > 0)
        this.pagedRequest.set({
          page: page,
          pageSize: this.pageSize,
          search: null,
        })

      return this.contractApi.getCompanyContracts(this.pagedRequest())
        .pipe(
          tap(res => {
            if(!res.data)
              return

            res.data.items.sort(
              (a, b) => CONTRACT_STATUS_ORDER[a.contractStatus] - CONTRACT_STATUS_ORDER[b.contractStatus]
            )

            this.pagedResponse.set(res.data);
            this.contracts.set(res.data.items);
          })
        )
    }
  getContractById(id: number) {
    this.request.set({
      state: 'loading',
      message: "Ładowanie kontraktów..."
    })
    return this.contractApi.getContractById(id)
      .pipe(
        tap(res => {
          if (res.data)
            this.selectedContract.set(res.data)

          this.request.set({
            state: 'success',
            message: `Pobrano Kontrakt ${id}`
          })
        }),
        catchError(error => {
          this.request.set({
            state: 'error',
            message: error.error.message
          })

          return throwError(() => error)
        })
      )
  }
  updateContract(id: number, form: EditContractDto) {
    return this.contractApi.updateContract(id, form)
      .pipe(
        tap(res => {
          this.request.set({
            state: 'success',
            message: res.message ?? "Zaktualizowano kontrakt"
          })
        })
      )
  }

  addContract(form: AddContractDto) {
    return this.contractApi.createContract(form)
      .pipe(
        tap(res => {
          this.request.set({
            state: 'success',
            message: res.message ?? "Dodano Kontrakt"
          })
        }),
        catchError(error => {
          this.request.set({
            state: 'error',
            message: error.error.message ?? "Błąd w danych formularza"
          })

          return throwError(() => error)
        })
      )
  }

  deleteContract(id: number) {
    return this.contractApi.deleteContractSoft(id)
      .pipe(
        tap(res => {
          console.log(res)

          this.contracts.update(contracts =>
            contracts.map(contract =>
              contract.contractId === id
                ? {
                  ...contract,
                  contractStatus: 'Cancelled'
                }
                : contract
            )
          )
        })
      )
  }

  getContractApplications(id: number) {
    return this.contractApi.getContractApplications(id)
      .pipe(
        tap(res => {
          if (res.data) {
            res.data.sort(
              (a, b) => APPLICATIONS_STATUS_ORDER[a.status] - APPLICATIONS_STATUS_ORDER[b.status]
            )
            this.applications.set(res.data)
          }
        })
      )
  }

  acceptApplication(applicationId: number, contractId: number) {
    return this.applicationApi.acceptApplication(applicationId)
      .pipe(
        tap(res => {
          this.request.set({
            state: 'success',
            message: res.message ?? "Dodano Kontrakt"
          })
        }),
        switchMap(() => this.getContractApplications(contractId)),
        catchError(error => {
          this.request.set({
            state: 'error',
            message: error.error.message ?? "Nie można zaakceptować tej samej aplikacji"
          })

          return throwError(() => error)
        })
      )
  }

  rejectApplication(applicationId: number) {
    return this.applicationApi.rejectApplication(applicationId)
      .pipe(
        tap(res => {
          this.applications.update(applications =>
            applications.map(app =>
              app.applicationId === applicationId
                ? {
                  ...app,
                  status: 'Rejected'
                }
                : app
            )
          );
          this.request.set({
            state: 'success',
            message: res.message ?? "Dodano Kontrakt"
          })
        }),
        catchError(error => {
          this.request.set({
            state: 'error',
            message: error.error.message ?? "Błąd odrzucenia"
          })

          return throwError(() => error)
        })
      )
  }
}
