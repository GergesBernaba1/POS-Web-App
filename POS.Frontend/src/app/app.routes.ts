import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { AuthGuard } from './guards/auth.guard';

// Lazy import for UserManagementComponent
const UserManagementComponent = () => import('./components/user-management/user-management.component').then(m => m.UserManagementComponent);

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: 'users', loadComponent: UserManagementComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: '/login' }
];
