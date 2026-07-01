import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from "@angular/router";

import { RoleType } from '../../features/auth/models/enums/role-type.enum';
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


  readonly currentUser = this.authStore.currentUser.asReadonly();
  
  readonly isSidebarCollapsed = this.layoutStore.isSidebarCollapsed.asReadonly();

  readonly roleType = RoleType;
  readonly isMobile = signal(window.innerWidth <= 740);

  readonly displayName = computed(() => {
    const user = this.currentUser();
    if(!user)
      return "Użytkownik";

    return user.role === this.roleType.company
      ? user.name
      : user.nickName
  })

  protected readonly baseRoute = computed(() => {
    return this.authStore.currentUser()?.role === this.roleType.company
    ? 'company'
    : 'pentester';
  })

  protected readonly navItems = computed(() => {
    const role = this.authStore.currentUser()?.role;

    if(role == this.roleType.company){
      return [
        { label: 'Dashboard', route: 'dashboard'},
        { label: 'Kontrakty', route: 'contracts'},
        { label: 'Raporty', route: 'reports'},
        { label: 'Profil', route: 'profile'},
        { label: 'Ustawienia', route: 'settings'}
      ]
    } else if(role == this.roleType.pentester){
      return [
        { label: 'Dashboard', route: 'dashboard'},
        { label: 'Oferty', route: 'offers'},
        { label: 'Zlecenia', route: 'assignments'},
        { label: 'Raporty', route: 'reports'},
        { label: 'Profil', route: 'profile'},
        { label: 'Ustawienia', route: 'settings'}
      ]
    } else {
      return
    }
  })

  constructor() {
  window.addEventListener('resize', () => {
    this.isMobile.set(window.innerWidth <= 740);
  });
}

  toogleSidebar() {
    this.layoutStore.toggleSidebar();
  }
  logout() {
    this.authStore.logout().subscribe();
  }
}
