import { Routes } from '@angular/router';

export const mainLayoutRoutes : Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: "full"
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./pages/dashboard-page/dashboard-page')
      .then(m => m.DashboardPage)
  }
]
