import { Component, inject } from '@angular/core';
import { PentesterDashboard } from '../../components/pentester-dashboard/pentester-dashboard';
import { CompanyDashboard } from '../../components/company-dashboard/company-dashboard';
import { RoleType } from '../../../auth/models/role-type.enum';
import { AuthStore } from '../../../auth/store/auth.store';

@Component({
  selector: 'app-dashboard-page',
  imports: [
    PentesterDashboard,
    CompanyDashboard
  ],
  templateUrl: './dashboard-page.html',
  styleUrl: './dashboard-page.scss',
})
export class DashboardPage {

  private readonly authStore = inject(AuthStore);
  readonly roleType = RoleType;

  readonly currentUser = this.authStore.currentUser;

}
