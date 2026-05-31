import { inject, Injectable, signal } from "@angular/core";
import { AuthApi } from "../../../core/services/auth-api";
import { LoginRequest } from "../models/login-request";
import { RegisterRequest } from "../models/register-request";

@Injectable({
  providedIn: 'root'
})
export class AuthStore
{
  private readonly authApiService = inject(AuthApi)

  readonly error = signal<string | null>(null);
  readonly success = signal<string | null>(null);
  readonly isLoading = signal<boolean>(false);


  login(request : LoginRequest)
  {
    this.clearSignals();
    this.isLoading.set(true);

    this.authApiService.login(request).subscribe({
      next: response => {
        this.isLoading.set(false);
        this.success.set(response.message);
      },
      error: error => {
        this.isLoading.set(false);
        this.error.set(error.error.message);
      }
    })
  }

  register(request: RegisterRequest)
  {
    this.clearSignals();
    this.isLoading.set(true);

    this.authApiService.register(request).subscribe({
      next: response => {
        this.isLoading.set(false);
        this.success.set(response.message)
      },
      error: error => {
        this.isLoading.set(false)
        this.error.set(error.error.message)
      }
    })
  }


  private clearSignals(){
    this.isLoading.set(false);
    this.error.set(null);
    this.success.set(null)
  }
}
