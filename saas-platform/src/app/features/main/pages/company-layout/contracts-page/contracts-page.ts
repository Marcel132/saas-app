import { Component, computed, inject, signal } from '@angular/core';
import { CompanyStore } from '../../../store/company.store';
import { ContractCard } from '../../../../../shared/components/contract-card/contract-card';

@Component({
  selector: 'app-contracts-page',
  imports: [
    ContractCard
  ],
  templateUrl: './contracts-page.html',
  styleUrl: './contracts-page.scss',
})
export class ContractsPage {
  // DI
  private readonly companyStore = inject(CompanyStore)

  // SIGNALS
  readonly contracts = this.companyStore.contracts

  ngOnInit(): void {
    this.companyStore.getContracts()
  }
}
