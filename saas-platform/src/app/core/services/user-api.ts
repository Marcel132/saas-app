import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiEndpoints } from '../constants/api-endpoints';
import { CurrentUserDto } from '../../features/auth/models/user-dto';

@Injectable({
  providedIn: 'root',
})
export class UserApi {

  private readonly http = inject(HttpClient)

  getCurrentUser(){
    return this.http.get<ApiResponseModel<CurrentUserDto>>(
      ApiEndpoints.users.currentUser,
      {withCredentials: true}
    )
  }
}
