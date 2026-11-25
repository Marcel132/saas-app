import { CanActivateFn, Router } from '@angular/router';
import { AuthStore } from '../../features/auth/store/auth.store';
import { inject } from '@angular/core';

export const guestGuard: CanActivateFn = (route, state) => {
  const authStore = inject(AuthStore);
  const router = inject(Router);
  const user = authStore.currentUser();

  if(!user) return true;

  console.log(`GuestGuard ${user}`)

  return router.createUrlTree([`/home`])
};
