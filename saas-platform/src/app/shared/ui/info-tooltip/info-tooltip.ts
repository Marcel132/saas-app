import { Component, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'ui-info-tooltip',
  imports: [
    MatIconModule,
    MatTooltipModule
  ],
  templateUrl: './info-tooltip.html',
  styleUrl: './info-tooltip.scss',
})
export class InfoTooltip {

  text = input.required<string>()

}
