import { inject, Injectable, signal } from "@angular/core";
import { ApplicationApi } from "../../../core/services/application-api";
import { catchError, switchMap, tap, throwError } from "rxjs";
import { ContractApi } from "../../../core/services/contract-api";
import { APPLICATIONS_STATUS_ORDER } from "../../../core/constants/application-status-order";
import { CompanyApplicationsDto } from "../models/company-applications-dto";
import { createRequestState } from "../../../core/utils/create-request-state";

@Injectable()

export class CompanyApplicationStore {
  private readonly applicationApi = inject(ApplicationApi);
  private readonly contractApi = inject(ContractApi);


  // SIGNALS
  private readonly _applicationRequest = createRequestState();
  readonly applicationRequest = this._applicationRequest.state;
  readonly clearApplicationRequest = this._applicationRequest.clear;

  private readonly _applications = signal<CompanyApplicationsDto[]>([]);
  readonly applications = this._applications.asReadonly();

  loadContractApplications(id: number) {
    this._applicationRequest.set('loading', "Ładowanie aplikacji...");
    return this.contractApi.getContractApplications(id)
      .pipe(
        tap(res => {
          if (res.data) {
            const sorted = [...res.data].sort(
              (a, b) => APPLICATIONS_STATUS_ORDER[a.status] - APPLICATIONS_STATUS_ORDER[b.status]
            )
            this._applications.set(sorted)
          }
        }),
        catchError(err => {
          this._applicationRequest.set('error', err?.error?.message);
          return throwError(() => err)
        })
      )
  }

  acceptApplication(applicationId: number, contractId: number) {
    this._applicationRequest.set('loading', "Akceptowanie...");
    return this.applicationApi.acceptApplication(applicationId)
      .pipe(
        switchMap(() =>
          this.loadContractApplications(contractId).pipe(
            tap(res => {
              this._applicationRequest.set('success', res.message);
            }),
          )
        ),
        catchError(err => {
          this._applicationRequest.set('error', err?.error?.message);
          return throwError(() => err);
        })
      )
  }

  rejectApplication(applicationId: number) {
    this._applicationRequest.set('loading', 'Odrzucanie aplikacji...')
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
          this._applicationRequest.set('success',res.message ?? "Odrzucono aplikację")
        }),
        catchError(err => {
          this._applicationRequest.set('error', err?.error?.message)

          return throwError(() => err)
        })
      )
  }
}
