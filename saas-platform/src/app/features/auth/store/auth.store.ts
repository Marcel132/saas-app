import { inject, Injectable, signal } from "@angular/core";
import { AuthApi } from "../../../core/services/auth-api";

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
    this.isLoading.set(true);
    this.error.set(null);

    this.authApiService.login(request).subscribe({
      next: response => {
        this.isLoading.set(false)
        this.success.set(response.message)
      },
      error: error => {
        this.isLoading.set(false)
        this.error.set(error.error.message)
      }
    })
  }
}
