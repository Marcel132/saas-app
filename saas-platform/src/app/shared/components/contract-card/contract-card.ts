import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe } from '../../pipes/currency-pipe';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-contract-card',
  imports: [
    RouterLink,
    CurrencyPipe,
    DatePipe
  ],
  templateUrl: './contract-card.html',
  styleUrl: './contract-card.scss',
})
export class ContractCard {
  title = input.required<string>();
  description = input.required<string>();
  price = input.required<number>();
  deadline = input.required<string>();
  route = input.required<string>();
  status = input.required<string>();
}
