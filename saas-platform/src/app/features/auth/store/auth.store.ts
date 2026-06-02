import { inject, Injectable, signal } from "@angular/core";
import { AuthApi } from "../../../core/services/auth-api";
import { LoginRequest } from "../models/login-request";
import { RegisterRequest } from "../models/register-request";
import { CurrentUserDto } from "../../main/models/user-dto";
import { UserApi } from "../../../core/services/user-api";
import { tap } from "rxjs/internal/operators/tap";

@Injectable({
  providedIn: 'root'
})
export class AuthStore
{
  private readonly authApiService = inject(AuthApi)
  private readonly userApiService = inject(UserApi)

  readonly error = signal<string | null>(null);
  readonly success = signal<string | null>(null);
  readonly isLoading = signal<boolean>(false);

  readonly currentUser = signal<CurrentUserDto | null>(null);

  // TODO: Change Subscribe into Observable

  login(request : LoginRequest)
  {
    this.clearSignals();
    this.isLoading.set(true);

    this.authApiService.login(request).subscribe({
      next: response => {
        this.isLoading.set(false);
        this.success.set(response.message);

        this.loadCurrentUser().subscribe();
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
        this.success.set(response.message);
        this.loadCurrentUser().subscribe();
      },
      error: error => {
        this.isLoading.set(false);
        this.error.set(error.error.message);
      }
    })
  };

    
  loadCurrentUser(){
    return this.userApiService.getCurrentUser().pipe(
      tap(response => {
        this.currentUser.set(response.data as CurrentUserDto);
      })
    )
  }

  logout(){
    this.authApiService.logout().subscribe({
      next: response => {
        this.currentUser.set(null);

      }
    })
  }

  private clearSignals(){
    this.isLoading.set(false);
    this.error.set(null);
    this.success.set(null)
  }
}
