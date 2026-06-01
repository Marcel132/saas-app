import { Component, signal } from '@angular/core';
import { RouterOutlet } from "@angular/router";

@Component({
  selector: 'app-main-layout',
  imports: [RouterOutlet],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss',
})
export class MainLayout {

  readonly isSidebarCollapsed = signal(false);

  toogleSidebar() {
    this.isSidebarCollapsed.update(value => !value);
  }
}
