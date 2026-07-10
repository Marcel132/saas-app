import { Component, computed, input, output, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe } from '../../../../../../../shared/pipes/currency-pipe';
import { DatePipe } from '@angular/common';
import { Badge } from "../../../../../../../shared/ui/badge/badge";
import { BadgeVariant } from '../../../../../../../shared/models/badge-variant';
import { ContractStatus } from '../../../../../../../shared/models/contract-status';
import { CompanyContractDto } from '../../../../../models/response/company-contract-dto';

@Component({
  selector: 'app-contract-card',
  imports: [
    RouterLink,
    CurrencyPipe,
    DatePipe,
    Badge,
  ],
  templateUrl: './contract-card.html',
  styleUrl: './contract-card.scss',
})
export class ContractCard {
  readonly isExpended = signal(false)
  readonly editClicked = output<number>();
  readonly deleteClicked = output<number>();


  contract = input.required<CompanyContractDto>();


  readonly variant = computed(() =>
    this.STATUS_VARIANTS[this.contract().contractStatus]
  )

  onEdit(event: Event) {
    event.stopPropagation();
    this.editClicked.emit(this.contract().contractId);
  }

  onDelete(event: Event) {
    event.stopPropagation()
    this.deleteClicked.emit(this.contract().contractId);
  }

  toggleDetails() {
    this.isExpended.update(x => !x);
  }

  STATUS_VARIANTS: Record<ContractStatus, BadgeVariant> = {
    Open: 'open',
    InProgress: 'in-progress',
    Completed: 'completed',
    Cancelled: 'cancelled'
  };

}
