import { Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from "@angular/router";
import { BreakpointObserver } from '@angular/cdk/layout';
import { toSignal } from '@angular/core/rxjs-interop';

import { RoleTypeValues } from '../../features/auth/models/enums/role-type.const';
import { AuthStore } from '../../features/auth/store/auth.store';
import { LayoutStore } from '../../core/stores/layout.store';
import { map } from 'rxjs';

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
  private readonly breakpointObserver = inject(BreakpointObserver);

  readonly currentUser = this.authStore.currentUser.asReadonly();

  readonly isSidebarCollapsed = this.layoutStore.isSidebarCollapsed.asReadonly();

  readonly roleType = RoleTypeValues;
  readonly isMobile = toSignal(
    this.breakpointObserver
      .observe('(max-width: 740px)')
      .pipe(map(res => res.matches)),
      {initialValue: window.innerWidth <= 740}
  );

  readonly displayName = computed(() => {
    const user = this.currentUser();
    if(!user)
      return "Użytkownik";

    return user.role === this.roleType.Company
      ? user.name
      : user.nickName
  })

  protected readonly baseRoute = computed(() => {
    return this.authStore.currentUser()?.role === this.roleType.Company
    ? 'company'
    : 'pentester';
  });

  private readonly companyLabelList = [
    { label: 'Dashboard', route: 'dashboard'},
    { label: 'Kontrakty', route: 'contracts'},
    { label: 'Raporty', route: 'reports'},
    { label: 'Profil', route: 'profile'},
    { label: 'Ustawienia', route: 'settings'}
  ];
  private readonly pentesterLabelList = [
    { label: 'Dashboard', route: 'dashboard'},
    { label: 'Oferty', route: 'offers'},
    { label: 'Zlecenia', route: 'assignments'},
    { label: 'Raporty', route: 'reports'},
    { label: 'Profil', route: 'profile'},
    { label: 'Ustawienia', route: 'settings'}
  ];

  protected readonly navItems = computed(() => {
    const role = this.authStore.currentUser()?.role;

    return role === this.roleType.Company
      ? this.companyLabelList
      : role === this.roleType.Pentester
        ? this.pentesterLabelList
        : [ { label: 'error', route: ''}]
  })

  toggleSidebar() {
    this.layoutStore.toggleSidebar();
  }
  onNavClickOnMobile() {
    if(this.isMobile()){
      this.toggleSidebar();
    }
  }
  logout() {
    this.authStore.logout().subscribe();
  }
}
