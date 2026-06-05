import { inject, Injectable, signal } from "@angular/core";
import { UserApi } from "../../../core/services/user-api";
import { switchMap, tap } from "rxjs";
import { UserSummaryDto } from "../models/user-summary-dto";
import { OffersDto } from "../models/offers-dto";
import { ContractApi } from "../../../core/services/contract-api";

@Injectable({
  providedIn: 'root'
})
export class MainStore{
  private readonly userApi = inject(UserApi)
  private readonly contractApi = inject(ContractApi)

  readonly summary = signal<UserSummaryDto | null>(null);
  readonly offers = signal<OffersDto[]>([]);
  readonly selectedOffer = signal<OffersDto | null>(null)


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

  loadOffers(){
    if(this.offers().length > 0)
      return

    this.contractApi.getContracts().pipe(
      tap(res => {
        console.log(res)
        if(!res.data)
          return;

        this.offers.set(res.data?.items)
      })
    ).subscribe()
  }

  loadOfferById(id: number){
    this.contractApi.getContractById(id).pipe(
      tap(res => {
        console.log(res)

        if(!res.data){
          return
        }

        this.selectedOffer.set(res.data)
      }
    )).subscribe()
  }

  sendApplication(id: number){
    return this.contractApi.createApplication(id).pipe(
      tap(() => console.log("Created application"))
    )
  }

}
