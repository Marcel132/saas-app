import { Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from "@angular/router";

import { RoleType } from '../../features/auth/models/role-type.enum';
import { AuthStore } from '../../features/auth/store/auth.store';
import { LayoutStore } from '../../core/stores/layout.store';

@Component({
  selector: 'app-main-layout',
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive
  ],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss',
})
export class MainLayout {
  private readonly authStore = inject(AuthStore);
  private readonly layoutStore = inject(LayoutStore);


  readonly currentUser = this.authStore.currentUser;
  readonly isSidebarCollapsed = this.layoutStore.isSidebarCollapsed;
  readonly roleType = RoleType;

  protected readonly baseRoute = computed(() => {
    return this.authStore.currentUser()?.role === this.roleType.company
    ? 'company'
    : 'pentester';
  })

  toogleSidebar() {
    this.layoutStore.toggleSidebar();
  }
  logout() {
    this.authStore.logout().subscribe();
  }

}
