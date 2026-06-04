import { Routes } from '@angular/router';
import { AuthLayout } from './layout/auth-layout/auth-layout';
import { MainLayout } from './layout/main-layout/main-layout';
import { PublicLayout } from './layout/public-layout/public-layout';
import { authGuard } from './core/guards/auth-guard';
import { guestGuard } from './core/guards/guest-guard';

export const routes: Routes = [
  {
    path: '',
    component: PublicLayout,
    loadChildren: () =>
      import('./features/public/public.routes')
      .then(m => m.publicLayoutRoutes)
  },
  {
    path: 'app',
    component: MainLayout,
    canActivate: [authGuard],
    loadChildren: () =>
      import('./features/main/main.routes')
    .then(m => m.mainLayoutRoutes)
  },
  {
    path: 'auth',
    component: AuthLayout,
    canActivate: [guestGuard],
    loadChildren: () =>
      import('./features/auth/auth.routes')
      .then(r => r.authRoutes)
  },
  {
    path: '**',
    redirectTo: 'app'
  }
];
