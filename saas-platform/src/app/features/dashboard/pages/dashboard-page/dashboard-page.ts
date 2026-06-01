import { Component, inject } from '@angular/core';
import { MainStore } from '../store/main.store';
import { RoleType } from '../../../auth/models/role-type.enum';
import { PentesterDashboard } from '../../components/pentester-dashboard/pentester-dashboard';
import { CompanyDashboard } from '../../components/company-dashboard/company-dashboard';

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

  private readonly mainStore = inject(MainStore);

  readonly currentUser = this.mainStore.currentUser;
  readonly roleType = RoleType;

  ngOnInit() {
    this.mainStore.loadCurrentUser()
  }
}
