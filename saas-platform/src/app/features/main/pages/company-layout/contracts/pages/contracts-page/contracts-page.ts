import { Component, computed, inject, signal } from '@angular/core';
import { CompanyStore } from '../../../../../store/company.store';
import { ContractCard } from '../../components/contract-card/contract-card';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-contracts-page',
  imports: [
    ContractCard,
    RouterLink
],
  templateUrl: './contracts-page.html',
  styleUrl: './contracts-page.scss',
})
export class ContractsPage {
  // DI
  private readonly companyStore = inject(CompanyStore)
  private readonly router = inject(Router)

  // SIGNALS
  readonly contracts = this.companyStore.contracts

  readonly counter = computed( () => ({
    open: this.contracts().filter(x => x.contractStatus == "Open").length,
    inProgress: this.contracts().filter(x => x.contractStatus == "InProgress").length,
    completed: this.contracts().filter(x => x.contractStatus == "Completed").length,
    cancelled: this.contracts().filter(x => x.contractStatus == "Cancelled").length
  }))

  ngOnInit(): void {
    this.companyStore.getContracts()
  }

  openEditPage(contractId: number) {
  this.router.navigate([
    '/app/company/contracts',
    contractId,
    'edit'
  ]);
}
}
