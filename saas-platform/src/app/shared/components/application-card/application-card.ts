import { Component, input } from '@angular/core';
import { CurrencyPipe } from '../../pipes/currency-pipe';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-application-card',
  imports: [
    CurrencyPipe,
    DatePipe
  ],
  templateUrl: './application-card.html',
  styleUrl: './application-card.scss',
})
export class ApplicationCard {
  title = input.required<string>()
  price = input.required<number>()
  status = input.required<string>()
  appliedAt = input.required<Date>()
}
