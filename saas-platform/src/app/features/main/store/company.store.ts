import { inject, Injectable, signal } from "@angular/core";
import { ContractDto } from "../models/contract-dto";
import { MeApi } from "../../../core/services/me-api";
import { ContractApi } from "../../../core/services/contract-api";
import { EditContractDto } from "../pages/company-layout/contracts/models/edit-contract-dto";
import { tap } from "rxjs";

@Injectable({
  providedIn: 'root'
})

export class CompanyStore{
  // DI
  private readonly meApi = inject(MeApi);
  private readonly contractApi = inject(ContractApi)

  // SIGNALS
  readonly isLoading = signal<boolean>(false)
  readonly contracts = signal<ContractDto[]>([])
  readonly selectedContract = signal<ContractDto | null>(null)

  getContracts(){
    this.isLoading.set(true)
    this.meApi.getContracts()
    .pipe()
    .subscribe({
      next: res =>  {
        if(res.data)
          this.contracts.set(res.data.items);

        this.isLoading.set(false)
        console.log(res)
      },
      error: err => {
        this.isLoading.set(false)
        console.log(err)
      }
    })
  }

  getContractById(id: number){
    this.isLoading.set(true)
    this.contractApi.getContractById(id).subscribe({
      next: res => {
        this.selectedContract.set(res.data)
        this.isLoading.set(false)
      },
      error: err => {
        this.isLoading.set(false)
        console.log(err)
      }
    })
  }

  saveContract(id: number, form: EditContractDto){
    return this.contractApi.updateContract(id, form).pipe(
      tap( res => console.log(res.message))
    )
  }
}
