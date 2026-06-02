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
  },
  {
    path: 'orders',
    loadComponent: () =>
      import('./pages/orders-page/orders-page')
      .then(o => o.OrdersPage)
  },
  {
    path: 'reports',
    loadComponent: () =>
      import('./pages/reports-page/reports-page')
      .then(r => r.ReportsPage)
  },
  {
    path: 'profile',
    loadComponent: () =>
      import("./pages/profile-page/profile-page")
      .then(p => p.ProfilePage)
  },
  {
    path: 'settings',
    loadComponent: () =>
      import('./pages/settings-page/settings-page')
      .then(s => s.SettingsPage)
  }
]
