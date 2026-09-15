import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { API_URL } from '../tokens/api-url';

export interface CurrentUser {
  id: string;
  email: string;
}

export interface RegisterResult {
  errors: string[];
}

interface AuthState {
  user: CurrentUser | null;
  checked: boolean;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = inject(API_URL);
  private readonly state = signal<AuthState>({ user: null, checked: false });

  readonly user = computed(() => this.state().user);
  readonly checked = computed(() => this.state().checked);

  async initialize(): Promise<void> {
    if (this.state().checked) return;
    try {
      const user = await firstValueFrom(this.http.get<CurrentUser>(`${this.apiUrl}/api/auth/current`));
      this.state.set({ user, checked: true });
    } catch {
      this.state.set({ user: null, checked: true });
    }
  }

  async login(email: string, password: string): Promise<void> {
    await firstValueFrom(this.http.post<void>(`${this.apiUrl}/api/auth/login`, { email, password }));
    const user = await firstValueFrom(this.http.get<CurrentUser>(`${this.apiUrl}/api/auth/current`));
    this.state.set({ user, checked: true });
  }

  async register(email: string, password: string): Promise<RegisterResult> {
    try {
      await firstValueFrom(this.http.post(`${this.apiUrl}/api/auth/register`, { email, password }));
      return { errors: [] };
    } catch (err: any) {
      const body = err.error;
      const errors: string[] = body?.errors?.length
        ? body.errors
        : body?.error
          ? [body.error]
          : ['Registration failed.'];
      return { errors };
    }
  }

  async logout(): Promise<void> {
    await firstValueFrom(this.http.post<void>(`${this.apiUrl}/api/auth/logout`, {}));
    this.state.set({ user: null, checked: true });
  }
}