import { Component, EventEmitter, Output } from '@angular/core';
import { RegisterCompanyRequest } from '../../../models/register-company-request';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AsYouType } from 'libphonenumber-js';
import { MatIcon } from "@angular/material/icon";

@Component({
  selector: 'app-company-form',
  imports: [
    ReactiveFormsModule,
    MatIcon
],
  templateUrl: './company-form.html',
  styleUrl: './company-form.scss',
})
export class CompanyForm {
  @Output()
  register = new EventEmitter<RegisterCompanyRequest>();

  private formatter = new AsYouType();
  showPassword: boolean = false;

  form = new FormGroup({
    email: new FormControl('', {
      validators: [
        Validators.required,
        Validators.email,
        Validators.maxLength(254)
      ],
      nonNullable: true
    }),
    password: new FormControl('', {
      validators: [
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(64),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).+$/)
      ],
      nonNullable: true
    }),
    name: new FormControl('', {
      validators: [
        Validators.required,
        Validators.maxLength(256)
      ],
      nonNullable: true
    }),
    nip: new FormControl('', {
      validators: [
        Validators.required,
        Validators.pattern(/^\d{10}$/),
        Validators.maxLength(10),
        Validators.minLength(10)
      ],
      nonNullable: true
    }),
    phone: new FormControl('', {
      validators: [
        Validators.required,
        Validators.maxLength(30)
      ],
      nonNullable: true
    }),
    city: new FormControl('', {
      validators: [
        Validators.required,
        Validators.maxLength(100)
      ],
      nonNullable: true
    }),
    country: new FormControl('', {
      validators: [
        Validators.required,
        Validators.maxLength(100)
      ],
      nonNullable: true
    }),
    postalCode: new FormControl('', {
      validators: [
        Validators.required,
        Validators.maxLength(20)
      ],
      nonNullable: true
    }),
    street: new FormControl('', {
      validators: [
        Validators.required,
        Validators.maxLength(100)
      ],
      nonNullable: true
    }),
    bio: new FormControl(null, {
      validators: [
        Validators.maxLength(1000)
      ]
    }),
    websiteUrl: new FormControl(null, {
      validators: [
        Validators.maxLength(256),
        Validators.pattern(/^(https?:\/\/)?([\w-]+(\.[\w-]+)+)(\/[\w-]*)*\/?$/)
      ]
    })
  });

  onPhoneInput(event: Event) {
    const input = event.target as HTMLInputElement;

    this.formatter.reset();

    input.value = this.formatter.input(input.value);
  }
  onSubmit(): void {

    if(this.form.invalid){
      this.form.markAllAsTouched();
      return;
    }

    this.register.emit(this.form.getRawValue());
  }
}
