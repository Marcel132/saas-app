import { Routes } from '@angular/router';
import { AuthLayout } from './layout/auth-layout/auth-layout';
import { MainLayout } from './layout/main-layout/main-layout';

export const routes: Routes = [
  {
    path: '',
    component: MainLayout,
    loadChildren: () =>
      import('./features/dashboard/main.routes')
    .then(m => m.mainLayoutRoutes)
  },
  {
    path: 'auth',
    component: AuthLayout,
    loadChildren: () =>
      import('./features/auth/auth.routes')
      .then(r => r.authRoutes)
  },
  {
    path: '**',
    redirectTo: 'auth',
    pathMatch: "full"
  }
];
