import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStore } from '../../features/auth/store/auth.store';

export const guestGuard: CanActivateFn = (route, state) => {
  const authStore = inject(AuthStore);
  const router = inject(Router);

  console.log('guard')
  console.log(authStore.currentUser());
  return !authStore.currentUser() ? true : router.createUrlTree([`app/${authStore.currentUser()?.role.toLowerCase()}`]);
};
