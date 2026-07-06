import { Component, effect, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from "@angular/forms";
import { AuthStore } from '../../store/auth.store';
import { Router, RouterLink } from '@angular/router';
import { Message } from "../../../../shared/ui/message/message";
import { CompanyForm } from './company-form/company-form';
import { PentesterForm } from './pentester-form/pentester-form';
import { RegisterCompanyRequest } from '../../models/register-company-request';
import { RegisterPentesterRequest } from '../../models/register-pentester-request';
import { RoleType, RoleTypeValues } from '../../models/enums/role-type.const';

@Component({
  selector: 'app-register-page',
  imports: [
    ReactiveFormsModule,
    Message,
    PentesterForm,
    CompanyForm,
    RouterLink
],
  templateUrl: './register-page.html',
  styleUrl: './register-page.scss',
})
export class RegisterPage {

  private readonly authStore = inject(AuthStore);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  readonly request = this.authStore.request.asReadonly();

  // readonly specializations = [
  //   {
  //     value: SpecializationType.WebSecurity,
  //     label: 'Web Security'
  //   },
  //   {
  //     value: SpecializationType.ApiSecurity,
  //     label: 'API Security'
  //   },
  //   {
  //     value: SpecializationType.MobileSecurity,
  //     label: 'Mobile Security'
  //   },
  //   {
  //     value: SpecializationType.CloudSecurity,
  //     label: 'Cloud Security'
  //   },
  //   {
  //     value: SpecializationType.InfrastructureSecurity,
  //     label: 'Infrastructure Security'
  //   },
  //   {
  //     value: SpecializationType.RedTeam,
  //     label: 'Red Team'
  //   }
  // ];
  constructor() {
    effect(() => {
      if (this.request().state == 'success')
        setTimeout(() => {
          this.router.navigate([`/app/${this.authStore.currentUser()?.role.toLowerCase()}`])
        }, 1000)
    })
  }

  ngOnInit() {
    this.authStore.clearRequestState();
  }

  form = this.fb.group({
    accountType: this.fb.control<RoleType>(RoleTypeValues.Pentester, { nonNullable: true })
  });

  onRegisterPentester(dto: RegisterPentesterRequest): void {
    console.log(dto)
    this.authStore.registerPentester(dto);
  }
  onRegisterCompany(dto: RegisterCompanyRequest): void {
    console.log(dto)
    this.authStore.registerCompany(dto);
  }

}

