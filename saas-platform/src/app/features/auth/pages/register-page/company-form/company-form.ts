import { Component, EventEmitter, Output } from '@angular/core';
import { RegisterCompanyRequest } from '../../../models/register-company-request';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-company-form',
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './company-form.html',
  styleUrl: './company-form.scss',
})
export class CompanyForm {
  @Output()
  register = new EventEmitter<RegisterCompanyRequest>();

  form = new FormGroup({
    email: new FormControl('', { nonNullable: true }),
    password: new FormControl('', { nonNullable: true }),
    name: new FormControl('', { nonNullable: true }),
    nip: new FormControl('', { nonNullable: true }),
    phone: new FormControl('', { nonNullable: true }),
    city: new FormControl('', { nonNullable: true }),
    country: new FormControl('', { nonNullable: true }),
    postalCode: new FormControl('', { nonNullable: true }),
    street: new FormControl('', { nonNullable: true }),
    bio: new FormControl(null, { nonNullable: false }),
    websiteUrl: new FormControl(null, { nonNullable: false })
  })

  onSubmit(): void {
    this.register.emit(this.form.getRawValue());
  }
}
