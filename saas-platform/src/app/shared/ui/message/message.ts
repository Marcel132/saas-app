import { Component, input } from '@angular/core';
import { RequestState } from '../../../core/utils/request-state';

@Component({
  selector: 'ui-message',
  imports: [],
  templateUrl: './message.html',
  styleUrl: './message.scss',
})
export class Message {

  request = input.required<RequestState>();

}
