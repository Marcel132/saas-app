import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { LayoutStore } from '../../core/stores/layout.store';

@Component({
  selector: 'app-public-layout',
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive
  ],
  templateUrl: './public-layout.html',
  styleUrl: './public-layout.scss',
})
export class PublicLayout {

  private readonly layoutStore = inject(LayoutStore);
  readonly isSidebarCollapsed = this.layoutStore.isSidebarCollapsed;

  toogleSidebar() {
    this.layoutStore.toggleSidebar();
  }
}
