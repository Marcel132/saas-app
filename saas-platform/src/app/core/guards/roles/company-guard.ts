import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthStore } from '../../../features/auth/store/auth.store';
import { RoleType } from '../../../features/auth/models/role-type.enum';

export const companyGuard: CanActivateFn = (route, state) => {
  const authStore = inject(AuthStore);

  return authStore.currentUser()?.role === RoleType.company
};
