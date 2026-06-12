import { Component, effect, inject } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { AuthStore } from '../../store/auth.store';
import { LoginRequest } from '../../models/login-request';
import { Message } from "../../../../shared/ui/message/message";

@Component({
  selector: 'app-login-page',
  imports: [
    ReactiveFormsModule,
    Message
],
  templateUrl: './login-page.html',
  styleUrl: './login-page.scss',
})
export class LoginPage {

  private readonly authStore = inject(AuthStore);
  private readonly router = inject(Router);
  readonly request = this.authStore.request

  constructor() {
    effect(() => {
      if (this.request().state == 'success') {
        setTimeout(() => {
          this.router.navigate([`app/${this.authStore.currentUser()?.role.toLowerCase()}`])
        }, 1000)
      }
    })
  }

  form = new FormGroup({
    email: new FormControl('', {
      nonNullable: true
    }),
    password: new FormControl('', {
      nonNullable: true
    })
  })

  login()
  {
    const request : LoginRequest =
    {
      Email: this.form.controls.email.value,
      Password: this.form.controls.password.value
    }

    this.authStore.login(request).subscribe()

  }
}
