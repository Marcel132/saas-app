import { Component, input, output, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { CreateRequestDto } from '../../models/create-request-dto';

@Component({
  selector: 'app-request-card',
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './request-card.html',
  styleUrl: './request-card.scss',
})
export class RequestCard {
  number = input.required<number>();
  save = output<CreateRequestDto>()
  showRequest = signal(false);

  form = new FormGroup({
    title: new FormControl('', {
      validators: [
        Validators.required,
        Validators.maxLength(256)
      ],
      nonNullable: true,
    }),
    url: new FormControl('', {
      validators: [
        Validators.required,
        Validators.maxLength(256),
        Validators.pattern(/^https?:\/\/.+/)
      ],
      nonNullable: true,
    }),
    scope: new FormControl('', {
      validators: [
        Validators.required,
        Validators.maxLength(1000)
      ],
      nonNullable: true
    }),
    credentials: new FormControl('Email: \nPassword:  ', {
      validators: [
        Validators.required,
        Validators.pattern(/^Email:\s*.+\nPassword:\s*.+$/)
      ],
      nonNullable: true
    }),
    deadline: new FormControl('', {
      validators: [
        Validators.required,
      ],
      nonNullable: true
    })
  })

  toggleRequest(){
    this.showRequest.update(x => !x)
  }

  createRequest(){
    if(this.form.invalid)
      return;

    this.save.emit(this.form.getRawValue())
  }
}
