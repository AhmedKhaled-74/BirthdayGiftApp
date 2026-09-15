import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-logout',
  imports: [RouterLink],
  templateUrl: './logout.html',
  styleUrl: './logout.css'
})
export class Logout implements OnInit, OnDestroy {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly phase = signal<'signing-out' | 'signed-out' | 'failed'>('signing-out');
  readonly countdown = signal(5);

  private timer: number | null = null;

  async ngOnInit(): Promise<void> {
    try {
      await this.auth.logout();
      this.phase.set('signed-out');
      this.startCountdown();
    } catch {
      this.phase.set('failed');
    }
  }

  ngOnDestroy(): void {
    this.clearCountdown();
  }

  staySignedOut(): void {
    this.clearCountdown();
    this.router.navigate(['/login']);
  }

  private startCountdown(): void {
    this.clearCountdown();
    this.timer = window.setInterval(() => {
      const next = this.countdown() - 1;
      this.countdown.set(next);
      if (next <= 0) {
        this.clearCountdown();
        this.router.navigate(['/login']);
      }
    }, 1000);
  }

  private clearCountdown(): void {
    if (this.timer !== null) {
      clearInterval(this.timer);
      this.timer = null;
    }
  }
}
