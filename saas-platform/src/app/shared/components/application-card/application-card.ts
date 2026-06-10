import { Component, input } from '@angular/core';
import { CurrencyPipe } from '../../pipes/currency-pipe';
import { DatePipe } from '@angular/common';
import { RouterLink } from "@angular/router";

@Component({
  selector: 'app-application-card',
  imports: [
    CurrencyPipe,
    DatePipe,
    RouterLink
],
  templateUrl: './application-card.html',
  styleUrl: './application-card.scss',
})
export class ApplicationCard {
  title = input.required<string>()
  price = input.required<number>()
  status = input.required<string>()
  appliedAt = input.required<Date>()
  route = input.required<string>()
}
