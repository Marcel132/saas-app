import { Component, input } from '@angular/core';
import { RouterLink } from "@angular/router";

@Component({
  selector: 'app-dashboard-card',
  imports: [
    RouterLink
  ],
  templateUrl: './dashboard-card.html',
  styleUrl: './dashboard-card.scss',
})
export class DashboardCard {
  title = input.required<string>()
  value = input.required<string | number>()
  route = input.required<string>()
  optionalValue = input<string | null>(null)
}
