import { Injectable, signal } from "@angular/core";

@Injectable({
  providedIn: 'root'
})
export class LayoutStore {
  readonly isSidebarCollapsed = signal(false);

  toggleSidebar() {
    this.isSidebarCollapsed.update(value => !value);
  }

  collapseSidebar() {
    this.isSidebarCollapsed.set(true);
  }
  expandSidebar() {
    this.isSidebarCollapsed.set(false);
  }
}
