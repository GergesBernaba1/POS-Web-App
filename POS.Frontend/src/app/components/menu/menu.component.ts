import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [RouterModule],
  template: `
    <nav class="main-menu">
      <a routerLink="/dashboard" routerLinkActive="active">Dashboard</a>
      <a routerLink="/users" routerLinkActive="active">User Management</a>
      <!-- Add more menu items as needed -->
    </nav>
  `,
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent {}
