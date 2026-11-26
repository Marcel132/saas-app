import { inject, Injectable, signal } from "@angular/core";
import { catchError, EMPTY, tap, throwError } from "rxjs";

import { ContractApi } from "../../../core/services/contract-api";
import { EditContractDto } from "../pages/company-layout/contracts/models/edit-contract-dto";
import { AddContractDto } from "../pages/company-layout/contracts/models/add-contract-dto";
import { CONTRACT_STATUS_ORDER } from "../../../core/constants/contract-status-order";
import { PagedRequestModel } from "../../../core/models/paged-request-model";
import { PagedResponseModel } from "../../../core/models/paged-response-model";
import { CompanyContractDto } from "../models/response/company-contract-dto";
import { ContractDetailsDto } from "../models/contracts/contract-details-dto";
import { CreateRequestDto } from "../pages/company-layout/contracts/models/create-request-dto";
import { ReportApi } from "../../../core/services/report-api";
import { createRequestState } from "../../../core/utils/create-request-state";

@Injectable()

export class CompanyStore {
  // DI
  private readonly contractApi = inject(ContractApi);
  private readonly reportApi = inject(ReportApi);

  // CONST
  private readonly pageSize = 4;
  private readonly page = 1;

  // PRIVATE SIGNALS
  private readonly _requestState = createRequestState()
  private readonly _contracts = signal<CompanyContractDto[]>([]);
  private readonly _selectedContract = signal<ContractDetailsDto | null>(null);
  private readonly _pagedRequest = signal<PagedRequestModel>({
    page: this.page,
    pageSize: this.pageSize,
    search: null
  });
  private readonly _pagedResponse = signal<PagedResponseModel<CompanyContractDto> | null>(null);

  // PUBLIC SIGNALS with flag asReadonly()
  readonly requestState = this._requestState.state;
  readonly contracts = this._contracts.asReadonly();
  readonly selectedContract = this._selectedContract.asReadonly();
  readonly pagedRequest = this._pagedRequest.asReadonly()
  readonly pagedResponse = this._pagedResponse.asReadonly()


  readonly clearRequestState = this._requestState.clear;

  loadContracts(force = false, page: number | null = null) {
    const isPageChanging = page !== null && page > 0 && page !== this._pagedRequest().page;

    if (!force && !isPageChanging && this._contracts().length > 0) {
      return EMPTY;
    }

    if (isPageChanging) {
      this._pagedRequest.update(req => ({ ...req, page }));
    }

    this._requestState.set('loading', 'Ładowanie kontraktów...');

    return this.contractApi.getCompanyContracts(this._pagedRequest()).pipe(
      tap(res => {
        if (!res.data) return;

        const sortedItems = [...res.data.items].sort(
          (a, b) => CONTRACT_STATUS_ORDER[a.contractStatus] - CONTRACT_STATUS_ORDER[b.contractStatus]
        );

        this._pagedResponse.set(res.data);
        this._contracts.set(sortedItems);
        this.clearRequestState();
      }),
      catchError(err => {
        this._requestState.set('error', err?.error?.message);
        return throwError(() => err);
      })
    );
  }

  loadContractById(id: number) {
    if (this._selectedContract()?.contractId !== id) {
      this._selectedContract.set(null);
    }

    this._requestState.set('loading', 'Ładowanie kontraktu...');

    return this.contractApi.getContractDetailsById(id).pipe(
      tap(res => {
        if (res.data) {
          this._selectedContract.set(res.data);
        }
        this.clearRequestState();
      }),
      catchError(err => {
        this._requestState.set('error', err?.error?.message);
        return throwError(() => err);
      })
    );
  }

  addContract(form: AddContractDto) {
    this._requestState.set('loading', "Dodawanie...")

    return this.contractApi.createContract(form)
      .pipe(
        tap(res => {
          let message: string = res.message ?? "Dodano kontrakt";
          this._requestState.set('success', message)
        }),
        catchError(err => {
          this._requestState.set('error', err?.error?.message)
          return throwError(() => err)
        })
      )
  }

  updateContract(id: number, form: EditContractDto) {
    this._requestState.set('loading', "Aktualizowanie...")
    return this.contractApi.updateContract(id, form)
      .pipe(
        tap(res => {
          let message: string = res.message ?? "Zaktualizowano kontrakt";
          this._requestState.set('success', message)
        }),
        catchError(err => {
          this._requestState.set('error', err?.error?.message);
          return throwError(() => err);
        })
      )
  }


  deleteContract(id: number) {
    this._requestState.set('loading', "Usuwanie...")
    return this.contractApi.deleteContractSoft(id)
      .pipe(
        tap(res => {
          this._requestState.set('success', "Usunieto kontrakt")
          this._contracts.update(contracts =>
            contracts.map(contract =>
              contract.contractId === id
                ? {
                  ...contract,
                  contractStatus: 'Cancelled'
                }
                : contract
            )
          )
        }),
        catchError(err => {
          this._requestState.set('error', err?.error?.message);
          return throwError(() => err)
        })
      )
  }

  createRequest(dto: CreateRequestDto) {
    this._requestState.set('loading', "Dodawanie...")

    return this.reportApi.createRequest(dto)
      .pipe(
        tap(res => {
          console.log(res)
          this._requestState.set('success', "Utworzono request")
        }),
        catchError(err => {
          this._requestState.set('error', err?.error?.message);
          return throwError(() => err)
        })
      )

  }
}
