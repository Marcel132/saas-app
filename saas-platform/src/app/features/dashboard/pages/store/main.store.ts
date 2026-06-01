import { inject, Injectable, signal } from '@angular/core';
import { CurrentUserDto } from '../../models/user-dto';
import { UserApi } from '../../../../core/services/user-api';

@Injectable({
  providedIn: 'root'
})
export class MainStore {

  readonly currentUser = signal<CurrentUserDto | null>(null);
  readonly isLoading = signal<boolean>(false);

  private readonly apiService = inject(UserApi);

  loadCurrentUser(){
    this.isLoading.set(true);

    this.apiService.getCurrentUser().subscribe({
      next: response => {
        this.isLoading.set(false);
      this. currentUser.set(response.data as CurrentUserDto);
        console.log(response.data)
      },
      error: error => {
        this.isLoading.set(false);
      this. currentUser.set(null);
      }
    })
  }

}
