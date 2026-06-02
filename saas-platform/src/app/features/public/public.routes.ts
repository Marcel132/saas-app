import { Routes } from '@angular/router';

export const publicLayoutRoutes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: "full"
  },
  {
    path: 'home',
    loadComponent: () =>
      import('./pages/home-page/home-page')
      .then(h => h.HomePage)
  },
  {
    path: 'about-us',
    loadComponent: () =>
      import('./pages/about-us-page/about-us-page')
      .then(au => au.AboutUsPage)
  },
  {
    path: 'orders',
    loadComponent: () =>
      import('./pages/public-orders-page/public-orders-page')
      .then(po => po.PublicOrdersPage)
  },
  {
    path: 'contact',
    loadComponent: () =>
      import('./pages/contact-page/contact-page')
      .then(c => c.ContactPage)
  }
]
