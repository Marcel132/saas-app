import { Component, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe } from '../../pipes/currency-pipe';
import { ContractStatus } from '../../models/contract-status';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-offer-card',
  imports: [
    RouterLink,
    CurrencyPipe,
    DatePipe
  ],
  templateUrl: './offer-card.html',
  styleUrl: './offer-card.scss',
})
export class OfferCard {
  title = input.required<string>();
  description = input.required<string>();
  pricePerRequest = input.required<number>();
  maxRequests = input.required<number>();
  maxBudget = input.required<number>();
  status = input.required<ContractStatus>();
  deadline = input.required<string>();
  hasApplied = input.required<boolean>();
  route = input.required<string>();

  isExpended = signal<boolean>(false);

  toggleDetails() {
    this.isExpended.update(x => !x);
  }
}
