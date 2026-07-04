import { Component, computed, inject, signal } from '@angular/core';
import { CompanyStore } from '../../../../../store/company.store';
import { ContractCard } from '../../components/contract-card/contract-card';
import { Router, RouterLink } from '@angular/router';
import { Badge } from "../../../../../../../shared/ui/badge/badge";
import { ContractStatus } from '../../../../../../../shared/models/contract-status';

@Component({
  selector: 'app-contracts-page',
  imports: [
    ContractCard,
    RouterLink,
    Badge
  ],
  templateUrl: './contracts-page.html',
  styleUrl: './contracts-page.scss',
})
export class ContractsPage {
  // DI
  private readonly companyStore = inject(CompanyStore)
  private readonly router = inject(Router)

  // SIGNALS
  readonly contracts = this.companyStore.contracts.asReadonly()
  // TODO: Move logic to backend
  selectedStatuses = signal<ContractStatus[]>([])
  filtredContracts = computed(() => {
    const selected = this.selectedStatuses();

    if (selected.length === 0)
      return this.contracts();

    return this.contracts().filter(c =>
      selected.includes(c.contractStatus)
    )
  })

  readonly counter = computed(() => ({
    open: this.contracts().filter(x => x.contractStatus == "Open").length,
    inProgress: this.contracts().filter(x => x.contractStatus == "InProgress").length,
    completed: this.contracts().filter(x => x.contractStatus == "Completed").length,
    cancelled: this.contracts().filter(x => x.contractStatus == "Cancelled").length
  }))

  readonly pages = computed(() => {
    const totalPages = this.companyStore.pagedResponse()?.totalPages ?? 0

    return Array.from(
      { length: totalPages },
      (_, i) => i + 1
    )
  })
  readonly currentPage = computed(
    () => this.companyStore.pagedResponse()?.page ?? 1
  );


  ngOnInit(): void {
    this.companyStore.getContracts()
      .subscribe()
  }
  toggleStatus(status: ContractStatus) {
    this.selectedStatuses.update(statuses => {
      if (statuses.includes(status)) {
        return statuses.filter(s => s !== status);
      }

      return [...statuses, status];
    });
  }

  openEditPage(contractId: number) {
    this.router.navigate([
      '/app/company/contracts',
      contractId,
      'edit'
    ]);
  }

  deleteMethod(contractId: number) {
    this.companyStore.deleteContract(contractId)
      .subscribe()
  }

  loadOfferByPage(pageNumber: number) {
    this.companyStore.loadOffers(true, pageNumber)
      .subscribe()
  }
}
