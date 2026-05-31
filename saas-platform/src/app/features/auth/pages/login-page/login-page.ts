import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { AuthStore } from '../../store/auth.store';
import { LoginRequest } from '../../models/login-request';

@Component({
  selector: 'app-login-page',
  imports: [
    ReactiveFormsModule,
  ],
  templateUrl: './login-page.html',
  styleUrl: './login-page.scss',
})
export class LoginPage {

  private readonly authStore = inject(AuthStore);
  readonly isLoading = this.authStore.isLoading;
  readonly error = this.authStore.error;
  readonly success = this.authStore.success

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

    this.authStore.login(request)
  }
}
