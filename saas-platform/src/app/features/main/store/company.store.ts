import { inject, Injectable, signal } from "@angular/core";
import { ContractDto } from "../models/contract-dto";
import { MeApi } from "../../../core/services/me-api";

@Injectable({
  providedIn: 'root'
})

export class CompanyStore{
  // DI
  private readonly meApi = inject(MeApi);

  // SIGNALS
  readonly contracts = signal<ContractDto[]>([])
  readonly selectedContract = signal<ContractDto | null>(null)

  getContracts(){
    this.meApi.getContracts()
    .pipe()
    .subscribe({
      next: res =>  {
        if(res.data)
          this.contracts.set(res.data.items);
        console.log(res)
      },
      error: err => console.log(err)
    })
  }
}
