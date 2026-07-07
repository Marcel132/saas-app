import { Component, input } from '@angular/core';

@Component({
  selector: 'app-request-card',
  imports: [],
  templateUrl: './request-card.html',
  styleUrl: './request-card.scss',
})
export class RequestCard {
  number = input.required<number>();
}
