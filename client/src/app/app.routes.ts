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
        path: 'drugs',
        loadComponent: () =>
          import('./features/drugs/drug-list.component').then(m => m.DrugListComponent),
      },
      { path: '', pathMatch: 'full', redirectTo: 'drugs' },
    ],
  },
  { path: '**', redirectTo: '' },
];
