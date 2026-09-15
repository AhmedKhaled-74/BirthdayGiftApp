import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { API_URL } from '../tokens/api-url';

export interface AgeSummary {
  years: number;
  months: number;
  days: number;
  daysUntilNextBirthday: number;
  isBirthday: boolean;
}

@Injectable({ providedIn: 'root' })
export class WelcomeService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = inject(API_URL);

  readonly summary = signal<AgeSummary | null>(null);
  readonly loading = signal<boolean>(false);
  readonly noBirthDate = signal<boolean>(false);
  readonly error = signal<string | null>(null);

  async load(): Promise<void> {
    this.loading.set(true);
    this.noBirthDate.set(false);
    this.error.set(null);
    this.summary.set(null);
    try {
      const timezone = Intl.DateTimeFormat().resolvedOptions().timeZone;
      const summary = await firstValueFrom(
        this.http.get<AgeSummary>(`${this.apiUrl}/api/welcome/summary`, { params: { timezone } })
      );
      this.summary.set(summary);
    } catch (err: any) {
      if (err?.status === 404) {
        this.noBirthDate.set(true);
      } else {
        this.error.set('Could not load your age summary. Please try again.');
      }
    } finally {
      this.loading.set(false);
    }
  }
}