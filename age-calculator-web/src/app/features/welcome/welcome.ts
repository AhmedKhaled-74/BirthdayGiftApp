import { Component, computed, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { AgeSummary, WelcomeService } from '../../core/services/welcome.service';

const COUNT_UP_DURATION_MS = 1000;

@Component({
  selector: 'app-welcome',
  imports: [RouterLink],
  templateUrl: './welcome.html',
  styleUrl: './welcome.css'
})
export class Welcome implements OnInit, OnDestroy {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly welcomeService = inject(WelcomeService);

  readonly user = this.auth.user;
  readonly summary = this.welcomeService.summary;
  readonly loading = this.welcomeService.loading;
  readonly noBirthDate = this.welcomeService.noBirthDate;
  readonly error = this.welcomeService.error;
  readonly isBirthday = computed(() => this.summary()?.isBirthday ?? false);
  readonly birthdayProgress = computed(() => {
    const s = this.summary();
    if (!s) return 0;
    // Approximate position in the yearly orbit: 0% just after a birthday, 100% on the day.
    if (s.isBirthday) return 100;
    const clamped = Math.min(365, Math.max(0, s.daysUntilNextBirthday));
    return Math.round(((365 - clamped) / 365) * 100);
  });

  readonly displayYears = signal(0);
  readonly displayMonths = signal(0);
  readonly displayDays = signal(0);
  readonly displayDaysUntilNextBirthday = signal(0);
  readonly leaving = signal(false);

  private countUpFrame: number | null = null;
  private readonly reduceMotion =
    typeof matchMedia !== 'undefined' &&
    matchMedia('(prefers-reduced-motion: reduce)').matches;

  async ngOnInit(): Promise<void> {
    await this.loadSummary();
  }

  ngOnDestroy(): void {
    this.cancelCountUp();
  }

  async retry(): Promise<void> {
    await this.loadSummary();
  }

  async logout(): Promise<void> {
    if (this.leaving()) return;
    this.leaving.set(true);
    // Let the exit motion land, then hand off to the Logout page which signs out.
    await new Promise((resolve) => setTimeout(resolve, this.reduceMotion ? 0 : 350));
    await this.router.navigate(['/logout']);
  }

  private async loadSummary(): Promise<void> {
    this.cancelCountUp();
    await this.welcomeService.load();
    if (this.noBirthDate()) {
      await this.router.navigate(['/profile']);
      return;
    }
    const current = this.summary();
    if (current) {
      this.reveal(current);
    }
  }

  private reveal(target: AgeSummary): void {
    if (this.reduceMotion) {
      this.showFinal(target);
      return;
    }
    const startedAt = performance.now();
    const tick = (now: number): void => {
      const progress = Math.min(1, (now - startedAt) / COUNT_UP_DURATION_MS);
      const eased = 1 - Math.pow(1 - progress, 3);
      this.displayYears.set(Math.round(target.years * eased));
      this.displayMonths.set(Math.round(target.months * eased));
      this.displayDays.set(Math.round(target.days * eased));
      this.displayDaysUntilNextBirthday.set(Math.round(target.daysUntilNextBirthday * eased));
      this.countUpFrame = progress < 1 ? requestAnimationFrame(tick) : null;
    };
    this.countUpFrame = requestAnimationFrame(tick);
  }

  private showFinal(target: AgeSummary): void {
    this.displayYears.set(target.years);
    this.displayMonths.set(target.months);
    this.displayDays.set(target.days);
    this.displayDaysUntilNextBirthday.set(target.daysUntilNextBirthday);
  }

  private cancelCountUp(): void {
    if (this.countUpFrame !== null) {
      cancelAnimationFrame(this.countUpFrame);
      this.countUpFrame = null;
    }
  }
}
