import { Routes } from '@angular/router';
import { AuthLayout } from './layout/auth-layout/auth-layout';

export const routes: Routes = [
  {
    path: 'auth',
    component: AuthLayout,
    loadChildren: () =>
      import('./features/auth/auth.routes')
      .then(r => r.authRoutes)
  }
  // {
  //   path: '',
  //   component: MainLayout,
  //   loadChildren: () =>
  //     import('./layout/main-layout/main-layout')
  // }
];
