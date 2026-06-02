import { inject, Injectable, signal } from '@angular/core';
import { CurrentUserDto } from '../models/user-dto';
import { UserApi } from '../../../core/services/user-api';
import { AuthApi } from '../../../core/services/auth-api';

@Injectable({
  providedIn: 'root'
})
export class MainStore {

  // readonly currentUser = signal<CurrentUserDto | null>(null);
  // readonly isLoading = signal<boolean>(false);

  // private readonly apiService = inject(UserApi);
  // private readonly authApi = inject(AuthApi);

  // loadCurrentUser(){
  //   this.isLoading.set(true);

  //   this.apiService.getCurrentUser().subscribe({
  //     next: response => {
  //       this.isLoading.set(false);
  //     this. currentUser.set(response.data as CurrentUserDto);
  //       console.log(response.data)
  //     },
  //     error: error => {
  //       this.isLoading.set(false);
  //     this. currentUser.set(null);
  //     }
  //   })
  // }

  // logoutUser(){
  //   this.authApi.logout().subscribe({
  //     next: response => {
  //       // TODO: Add notify for successful logout
  //       this.currentUser.set(null);
  //     },
  //     error: error => {
  //       // TODO: Add notify for logout error
  //     }
  //   })
  // }

}
