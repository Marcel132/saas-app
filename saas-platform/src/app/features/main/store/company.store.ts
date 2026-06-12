import { inject, Injectable, signal } from "@angular/core";
import { ContractDto } from "../models/contract-dto";
import { MeApi } from "../../../core/services/me-api";
import { ContractApi } from "../../../core/services/contract-api";
import { EditContractDto } from "../pages/company-layout/contracts/models/edit-contract-dto";
import { tap } from "rxjs";
import { AddContractDto } from "../pages/company-layout/contracts/models/add-contract-dto";

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

    const statusOrder = {
      InProgress: 0,
      Open: 1,
      Completed: 2,
      Cancelled: 3,
    }
    this.isLoading.set(true)
    this.meApi.getContracts()
    .pipe()
    .subscribe({
      next: res =>  {
        if(res.data){
          res.data.items.sort(
            (a,b) => statusOrder[a.contractStatus] - statusOrder[b.contractStatus]
          )
          this.contracts.set(res.data.items);
        }

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

  updateContract(id: number, form: EditContractDto){
    return this.contractApi.updateContract(id, form).pipe(
      tap( res => console.log(res.message))
    )
  }

  addContract(form: AddContractDto){
    return this.contractApi.craeteContract(form).pipe()
  }

  deleteContract(id: number){
    this.contractApi.deleteContract(id).pipe(
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
    ).subscribe()
  }
}
