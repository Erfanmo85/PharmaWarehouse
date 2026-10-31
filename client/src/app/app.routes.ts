import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./features/login/login.component').then(m => m.LoginComponent),
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./layout/shell.component').then(m => m.ShellComponent),
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
      },
      {
        path: 'drugs',
        loadComponent: () =>
          import('./features/drugs/drug-list.component').then(m => m.DrugListComponent),
      },
      {
        path: 'drugs/new',
        loadComponent: () =>
          import('./features/drugs/drug-form.component').then(m => m.DrugFormComponent),
      },
      {
        path: 'drugs/:id',
        loadComponent: () =>
          import('./features/drugs/drug-detail.component').then(m => m.DrugDetailComponent),
      },
      {
        path: 'dispatch',
        loadComponent: () =>
          import('./features/stock/dispatch.component').then(m => m.DispatchComponent),
      },
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
    ],
  },
  { path: '**', redirectTo: '' },
];
