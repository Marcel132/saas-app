import { Component, input } from '@angular/core';

@Component({
  selector: 'app-contract-card',
  imports: [],
  templateUrl: './contract-card.html',
  styleUrl: './contract-card.scss',
})
export class ContractCard {
  title = input.required<string>();
  description = input.required<string>();
}
