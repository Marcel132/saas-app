import { CanActivateFn, Router } from '@angular/router';
import { RoleType } from '../../features/auth/models/enums/role-type.const';
import { inject } from '@angular/core';
import { AuthStore } from '../../features/auth/store/auth.store';

export const roleGuard = (allowedRole: RoleType): CanActivateFn => {
  return () => {
    const authStore = inject(AuthStore);
    const router = inject(Router);
    const user = authStore.currentUser();

    if(!user){
      return router.createUrlTree(['/home'])
    }

    if(user.role === allowedRole) return true;

    return router.createUrlTree([`/app/${user.role.toLowerCase()}`]);
  }
}
