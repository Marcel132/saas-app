import { Component, computed, effect, input, output, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe } from '../../../../../../../shared/pipes/currency-pipe';
import { DatePipe, JsonPipe } from '@angular/common';
import { Badge } from "../../../../../../../shared/ui/badge/badge";
import { BadgeVariant } from '../../../../../../../shared/models/badge-variant';
import { ContractStatus } from '../../../../../../../shared/models/contract-status';

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


  contractId = input.required<number>();
  title = input.required<string>();
  description = input.required<string>();
  pricePerRequest = input.required<number>();
  maxBudget = input.required<number>();
  maxRequests = input.required<number>();
  status = input.required<ContractStatus>();
  createdAt = input.required<string>()
  numberOfApplications = input.required<number>();
  deadline = input.required<string>();
  route = input.required<string>();

  readonly variant = computed(() =>
    this.STATUS_VARIANTS[this.status()]
  )


  constructor() {
    effect(() => {
      console.log(this.numberOfApplications());
    });
  }
  onEdit(event: Event) {
    event.stopPropagation();
    this.editClicked.emit(this.contractId());
  }

  onDelete(event: Event) {
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
