import { Component, computed, input, output, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe } from '../../../../../../../shared/pipes/currency-pipe';
import { DatePipe } from '@angular/common';
import { Badge } from "../../../../../../../shared/ui/badge/badge";
import { BadgeVariant } from '../../../../../../../shared/models/badge-variant';
import { ContractStatus } from '../../../../../../../shared/models/contract-status';

@Component({
  selector: 'app-contract-card',
  imports: [
    RouterLink,
    CurrencyPipe,
    DatePipe,
    Badge
],
  templateUrl: './contract-card.html',
  styleUrl: './contract-card.scss',
})
export class ContractCard {
  readonly isExpended = signal(false)

  contractId = input.required<number>();
  readonly editClicked = output<number>();
  readonly deleteClicked = output<number>();

  title = input.required<string>();
  description = input.required<string>();
  price = input.required<number>();
  deadline = input.required<string>();
  route = input.required<string>();
  status = input.required<ContractStatus>();

  readonly variant = computed(() =>
    this.STATUS_VARIANTS[this.status()]
  )

  onEdit(event: Event){
    event.stopPropagation();
    this.editClicked.emit(this.contractId());
  }

  onDelete(event: Event){
    event.stopPropagation()
    this.deleteClicked.emit(this.contractId());
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
