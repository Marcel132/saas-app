import { Component, effect, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthStore } from '../../store/auth.store';
import { LoginRequest } from '../../models/login-request';
import { Message } from "../../../../shared/ui/message/message";

@Component({
  selector: 'app-login-page',
  imports: [
    ReactiveFormsModule,
    Message,
    RouterLink
],
  templateUrl: './login-page.html',
  styleUrl: './login-page.scss',
})
export class LoginPage {

  private readonly authStore = inject(AuthStore);
  private readonly router = inject(Router);
  readonly request = this.authStore.request.asReadonly()

  constructor() {
    effect(() => {
      if (this.request().state == 'success') {
        setTimeout(() => {
          this.router.navigate([`app/${this.authStore.currentUser()?.role.toLowerCase()}`])
        }, 1000)
      }
    })
  }

  ngOnInit() {
    this.authStore.clearRequestState();
  }

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
    })
  })

  login() {
    const request: LoginRequest =
    {
      Email: this.form.controls.email.value,
      Password: this.form.controls.password.value
    }

    if(this.form.invalid)
      return;
    this.authStore.login(request)
      .subscribe()
  }
}
