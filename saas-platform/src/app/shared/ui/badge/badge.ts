import { Component, input } from '@angular/core';
import { BadgeVariant } from '../../models/badge-variant';

@Component({
  selector: 'app-badge',
  imports: [],
  templateUrl: './badge.html',
  styleUrl: './badge.scss',
})
export class Badge {
  variant = input.required<BadgeVariant>();
  text = input.required<string>();
}
