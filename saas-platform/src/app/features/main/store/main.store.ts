import { inject, Injectable, signal } from "@angular/core";
import { UserApi } from "../../../core/services/user-api";
import { switchMap, tap } from "rxjs";
import { UserSummaryDto } from "../models/user-summary-dto";
import { OffersDto } from "../models/offers-dto";
import { ContractApi } from "../../../core/services/contract-api";
import { ApplicationDto } from "../models/application-dto";
import { MeApi } from "../../../core/services/me-api";

@Injectable({
  providedIn: 'root'
})
export class MainStore{

  // DI
  private readonly userApi = inject(UserApi)
  private readonly contractApi = inject(ContractApi)
  private readonly meApi = inject(MeApi)

  // STATES
  readonly summary = signal<UserSummaryDto | null>(null);
  readonly offers = signal<OffersDto[]>([]);
  readonly selectedOffer = signal<OffersDto | null>(null)
  readonly userApplications = signal<ApplicationDto[]>([])

  // -----------
  // DASHBOARD PAGE
  // -----------

  loadSummary(){
    if(this.summary())
      return;
    this.userApi.getCurrentUserSummary().pipe(
      tap(res => {
        console.log(res)
        this.summary.set(res.data)
      }),
    ).subscribe()
  }

  // -----------
  // OFFERS PAGE
  // -----------

  loadOffers(){
    if(this.offers().length > 0)
      return

    this.contractApi.getContracts()
      .pipe()
      .subscribe({
        next: res => this.offers.set(res.data!.items),
        error: err => console.log(err)
      })
  }

  loadOfferById(id: number){
    return this.contractApi.getContractById(id).pipe(
      tap(res => {
        console.log(res)

        if(!res.data){
          return
        }

        this.selectedOffer.set(res.data)
      }
    ))
  }

  sendApplication(id: number){
    return this.contractApi.createApplication(id).pipe(
      switchMap(() => this.contractApi.getContractById(id)),
      tap(res => {
        if(res.data)
          this.selectedOffer.set(res.data)
      })
    )
  }

  // -----------
  // ASSIGNMENTS PAGE
  // -----------

  loadApplications(){
    return this.meApi.getApplications().pipe(
      tap(res => {
        console.log(res)

        if(res.data)
            this.userApplications.set(res.data)
      }),

    )
  }
}
