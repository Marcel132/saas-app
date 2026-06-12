import { Component, input } from '@angular/core';
import { RequestState } from '../../../core/models/request-state';

@Component({
  selector: 'app-message',
  imports: [],
  templateUrl: './message.html',
  styleUrl: './message.scss',
})
export class Message {

  request = input.required<RequestState>();

}
