import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { ApiEndpoints } from '../constants/api-endpoints';
import { UserSummaryDto } from '../../features/main/models/response/user-summary-dto';
import { ApiResponseModel } from '../models/api-response-model';
import { CurrentUser } from '../../features/auth/models/current-user/current-user-base';

@Injectable({
  providedIn: 'root',
})
export class UserApi {

  private readonly http = inject(HttpClient)

  getCurrentUser() {
    return this.http.get<ApiResponseModel<CurrentUser>>(
      ApiEndpoints.users.currentUser,
    )
  }

  getCurrentUserSummary() {
    return this.http.get<ApiResponseModel<UserSummaryDto>>(
      ApiEndpoints.users.summary,
    )
  }
}
