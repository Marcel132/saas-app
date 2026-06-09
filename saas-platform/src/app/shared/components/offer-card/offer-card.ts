import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe } from '../../pipes/currency-pipe';

@Component({
  selector: 'app-offer-card',
  imports: [
    RouterLink,
    CurrencyPipe
  ],
  templateUrl: './offer-card.html',
  styleUrl: './offer-card.scss',
})
export class OfferCard {
  title = input.required<string>();
  description = input.required<string>();
  price = input.required<number>()
  route = input.required<string>()
  hasApplied = input.required<boolean>()
}
