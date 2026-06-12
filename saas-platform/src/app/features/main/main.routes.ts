import { Routes } from '@angular/router';

import { companyGuard } from '../../core/guards/roles/company-guard';
import { pentesterGuard } from '../../core/guards/roles/pentester-guard';

export const mainLayoutRoutes: Routes = [
  {
    path: 'company',
    canActivate: [companyGuard],
    loadComponent: () =>
      import('./pages/company-layout/company-layout')
        .then(c => c.CompanyLayout),
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./pages/company-layout/dashboard-page/dashboard-page')
            .then(x => x.DashboardPage)
      },
      {
        path: 'assignments',
        loadComponent: () =>
          import('./pages/company-layout/assignments-page/assignments-page')
            .then(x => x.AssignmentsPage)
      },
      {
        path: 'contracts',
        loadComponent: () =>
          import('./pages/company-layout/contracts/pages/contracts-page/contracts-page')
            .then(x => x.ContractsPage),
      },
      {
        path: 'contracts/:id/edit',
        loadComponent: () =>
          import('./pages/company-layout/contracts/pages/edit-contract-page/edit-contract-page')
            .then(x => x.EditContractPage)
      },
      {
        path: 'contracts/:id/edit',
        loadComponent: () =>
          import('./pages/company-layout/contracts/pages/edit-contract-page/edit-contract-page')
            .then(x => x.EditContractPage)
      },
      {
        path: 'contracts/add',
        loadComponent: () =>
          import('./pages/company-layout/contracts/pages/add-contract-page/add-contract-page')
            .then(x => x.AddContractPage)
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('./pages/company-layout/profile-page/profile-page')
            .then(x => x.ProfilePage)
      },
      {
        path: 'reports',
        loadComponent: () =>
          import('./pages/company-layout/reports-page/reports-page')
            .then(x => x.ReportsPage)
      },
      {
        path: 'settings',
        loadComponent: () =>
          import('./pages/company-layout/settings-page/settings-page')
            .then(x => x.SettingsPage)
      }
    ]
  },
  {
    path: 'pentester',
    canActivate: [pentesterGuard],
    loadComponent: () =>
      import('./pages/pentester-layout/pentester-layout')
        .then(p => p.PentesterLayout),
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./pages/pentester-layout/dashboard-page/dashboard-page')
            .then(x => x.DashboardPage)
      },
      {
        path: 'assignments',
        loadComponent: () =>
          import('./pages/pentester-layout/assignments-page/assignments-page')
            .then(x => x.AssignmentsPage)
      },
      {
        path: 'offers',
        loadComponent: () =>
          import('./pages/pentester-layout/offers-page/offers-page')
            .then(x => x.OffersPage)
      },
      {
        path: 'offers/:id',
        loadComponent: () =>
          import('./pages/pentester-layout/offers-details-page/offers-details-page')
            .then(x => x.OffersDetailsPage)
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('./pages/pentester-layout/profile-page/profile-page')
            .then(x => x.ProfilePage)
      },
      {
        path: 'reports',
        loadComponent: () =>
          import('./pages/pentester-layout/reports-page/reports-page')
            .then(x => x.ReportsPage)
      },
      {
        path: 'settings',
        loadComponent: () =>
          import('./pages/pentester-layout/settings-page/settings-page')
            .then(x => x.SettingsPage)
      }
    ]
  }
]
