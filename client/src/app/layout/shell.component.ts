import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { AuthService } from '../core/services/auth.service';

@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatToolbarModule, MatButtonModule, MatIconModule],
  template: `
    <mat-toolbar>
      <span class="title">انبار دارو</span>
      <a mat-button routerLink="/drugs" routerLinkActive="active">داروها</a>
      <span class="spacer"></span>
      <button mat-button (click)="auth.logout()">
        <mat-icon>logout</mat-icon>
        خروج
      </button>
    </mat-toolbar>

    <main class="content">
      <router-outlet />
    </main>
  `,
  styles: [`
    .title { font-weight: 700; margin-inline-end: 16px; }
    .spacer { flex: 1 1 auto; }
    .active { font-weight: 700; }
    .content { padding: 24px; max-width: 1100px; margin: 0 auto; }
  `],
})
export class ShellComponent {
  protected readonly auth = inject(AuthService);
}
