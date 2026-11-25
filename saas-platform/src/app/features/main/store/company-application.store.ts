import { inject, Injectable, signal } from "@angular/core";
import { ApplicationApi } from "../../../core/services/application-api";
import { RequestState } from "../../../core/models/request-state";
import { catchError, switchMap, tap, throwError } from "rxjs";
import { ContractApi } from "../../../core/services/contract-api";
import { APPLICATIONS_STATUS_ORDER } from "../../../core/constants/application-status-order";
import { CompanyApplicationsDto } from "../models/company-applications-dto";

@Injectable()

export class CompanyApplicationStore {
  private readonly applicationApi = inject(ApplicationApi);
  private readonly contractApi = inject(ContractApi);


  // SIGNALS

  private readonly _applicationRequest = signal<RequestState>({
    state: 'idle',
    message: ''
  });
  readonly applicationRequest = this._applicationRequest.asReadonly();

  private readonly _applications = signal<CompanyApplicationsDto[]>([]);
  readonly applications = this._applications.asReadonly();

  getContractApplications(id: number) {
    return this.contractApi.getContractApplications(id)
      .pipe(
        tap(res => {
          if (res.data) {
            [...res.data].sort(
              (a, b) => APPLICATIONS_STATUS_ORDER[a.status] - APPLICATIONS_STATUS_ORDER[b.status]
            )
            this._applications.set(res.data)
          }
        })
      )
  }

  acceptApplication(applicationId: number, contractId: number) {
    return this.applicationApi.acceptApplication(applicationId)
      .pipe(
        switchMap(() => this.getContractApplications(contractId)),
        tap(res => {
          this._applicationRequest.set({
            state: 'success',
            message: res.message ?? "Zaakceptowano Aplikację"
          })
        }),
        catchError(err => {
          this._applicationRequest.set({
            state: 'error',
            message: err?.error?.message ?? 'Wystąpił nieoczekiwany błąd'
          })

          return throwError(() => err)
        })
      )
  }

  rejectApplication(applicationId: number) {
    return this.applicationApi.rejectApplication(applicationId)
      .pipe(
        tap(res => {
          this._applications.update(applications =>
            applications.map(app =>
              app.applicationId === applicationId
                ? {
                  ...app,
                  status: 'Rejected'
                }
                : app
            )
          );
          this._applicationRequest.set({
            state: 'success',
            message: res.message ?? "Odrzucono aplikację"
          })
        }),
        catchError(err => {
          this._applicationRequest.set({
            state: 'error',
            message: err?.error?.message ?? 'Wystąpił nieoczekiwany błąd'
          })

          return throwError(() => err)
        })
      )
  }

  clearRequestState() {
    this._applicationRequest.set({
      state: 'idle',
      message: ''
    })
  }
}
