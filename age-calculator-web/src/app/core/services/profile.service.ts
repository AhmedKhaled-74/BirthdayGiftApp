import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { API_URL } from '../tokens/api-url';

export interface Profile {
  id: string;
  email: string;
  birthDate: string | null;
}

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = inject(API_URL);

  readonly profile = signal<Profile | null>(null);
  readonly loading = signal<boolean>(false);
  readonly error = signal<string | null>(null);

  async load(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      const profile = await firstValueFrom(this.http.get<Profile>(`${this.apiUrl}/api/profile`));
      this.profile.set(profile);
    } catch {
      this.error.set('Could not load your profile. Please try again.');
    } finally {
      this.loading.set(false);
    }
  }

  async saveBirthDate(birthDate: string): Promise<void> {
    const profile = await firstValueFrom(
      this.http.put<Profile>(`${this.apiUrl}/api/profile/birth-date`, { birthDate })
    );
    this.profile.set(profile);
  }
}