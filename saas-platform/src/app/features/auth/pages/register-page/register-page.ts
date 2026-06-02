import { Component, effect, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { AuthStore } from '../../store/auth.store';
import { toSignal } from '@angular/core/rxjs-interop';
import { RoleType } from '../../models/role-type.enum';
import { SpecializationType } from '../../models/specialization-type.enum';
import { RegisterRequest } from '../../models/register-request';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register-page',
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './register-page.html',
  styleUrl: './register-page.scss',
})
export class RegisterPage {

  private readonly authStore = inject(AuthStore);
  private readonly router = inject(Router);

  readonly isLoading = this.authStore.isLoading;
  readonly error = this.authStore.error;
  readonly success = this.authStore.success;

  readonly roleType = RoleType;
  readonly specializationType = SpecializationType;

  readonly specializations = [
  {
    value: SpecializationType.WebSecurity,
    label: 'Web Security'
  },
  {
    value: SpecializationType.ApiSecurity,
    label: 'API Security'
  },
  {
    value: SpecializationType.MobileSecurity,
    label: 'Mobile Security'
  },
  {
    value: SpecializationType.CloudSecurity,
    label: 'Cloud Security'
  },
  {
    value: SpecializationType.InfrastructureSecurity,
    label: 'Infrastructure Security'
  },
  {
    value: SpecializationType.RedTeam,
    label: 'Red Team'
  }
  ];

  constructor() {
    effect(() => {
      if(this.authStore.success() !== null)
        setTimeout(() => {
          this.router.navigate(['/app'])
        }, 1000)
    })
  }

  form = new FormGroup({
    email: new FormControl('', { nonNullable: true }),
    password: new FormControl('', { nonNullable: true }),

    accountType: new FormControl<RoleType>(RoleType.pentester, { nonNullable: true }),
    specializations: new FormControl<SpecializationType[]>([], {nonNullable: true}),

    firstname: new FormControl('', { nonNullable: true }),
    lastname: new FormControl('', { nonNullable: true }),
    description: new FormControl('', { nonNullable: true }),
    nickname: new FormControl('', { nonNullable: true }),
    phone: new FormControl('', { nonNullable: true }),

    city: new FormControl('', { nonNullable: true }),
    country: new FormControl('', { nonNullable: true }),
    postalCode: new FormControl('', { nonNullable: true }),
    street: new FormControl('', { nonNullable: true }),

    companyName: new FormControl('', { nonNullable: true }),
    companyNip: new FormControl('', { nonNullable: true }),
  });

  readonly accountType = toSignal(
    this.form.controls.accountType.valueChanges,
    {
      initialValue: this.form.controls.accountType.value
    }
  );

  register(){

    if(this.accountType() != this.roleType.company)
      this.form.patchValue({
        companyName: '',
        companyNip: ''
      })
    if(this.accountType() != this.roleType.pentester)
      this.form.patchValue({
        specializations: []
    })

    const request: RegisterRequest = {
      Email: this.form.controls.email.value,
      Password: this.form.controls.password.value,
      Role: this.form.controls.accountType.value,
      SpecializationType: this.form.controls.specializations.value,
      Description: this.form.controls.description.value,
      FirstName: this.form.controls.firstname.value,
      LastName: this.form.controls.lastname.value,
      Nickname: this.form.controls.nickname.value,
      PhoneNumber: this.form.controls.phone.value,
      City: this.form.controls.city.value,
      Country: this.form.controls.country.value,
      PostalCode: this.form.controls.postalCode.value,
      Street: this.form.controls.street.value,
      CompanyName: this.form.controls.companyName.value,
      CompanyNip: this.form.controls.companyNip.value,
    }

    this.authStore.register(request)
  };

  toggleSpecialization(
  specialization: SpecializationType,
  event: Event
  ): void {

    const checked = (event.target as HTMLInputElement).checked;

    const current = [...this.form.controls.specializations.value];

    if (checked) {
      current.push(specialization);
    } else {
      const index = current.indexOf(specialization);

      if (index >= 0) {
        current.splice(index, 1);
      }
    }

    this.form.controls.specializations.setValue(current);
  }


}

