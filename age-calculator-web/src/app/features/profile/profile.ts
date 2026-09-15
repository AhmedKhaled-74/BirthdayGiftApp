import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { ProfileService } from '../../core/services/profile.service';

@Component({
  selector: 'app-profile',
  imports: [ReactiveFormsModule],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class Profile implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly auth = inject(AuthService);
  private readonly profileService = inject(ProfileService);
  private readonly router = inject(Router);

  readonly user = this.auth.user;
  readonly profile = this.profileService.profile;
  readonly loading = this.profileService.loading;
  readonly loadError = this.profileService.error;

  readonly form = this.fb.group({
    birthDate: ['', [Validators.required]]
  });

  editing = false;
  errorMessage = '';
  successMessage = '';
  saving = false;
  readonly leaving = signal(false);

  async ngOnInit(): Promise<void> {
    await this.reload();
  }

  async reload(): Promise<void> {
    await this.profileService.load();
    const bd = this.profile()?.birthDate;
    if (bd) {
      this.form.controls.birthDate.setValue(bd.split('T')[0]);
    }
  }

  startEdit(): void {
    this.editing = true;
    this.successMessage = '';
    this.errorMessage = '';
  }

  cancel(): void {
    this.editing = false;
    this.errorMessage = '';
    const bd = this.profile()?.birthDate;
    if (bd) {
      this.form.controls.birthDate.setValue(bd.split('T')[0]);
    } else {
      this.form.controls.birthDate.reset();
    }
  }

  async save(): Promise<void> {
    this.errorMessage = '';
    this.successMessage = '';
    if (this.form.invalid || this.saving) return;

    this.saving = true;
    try {
      await this.profileService.saveBirthDate(this.form.controls.birthDate.value);
      this.editing = false;
      this.successMessage = 'Birth date saved.';
    } catch {
      this.errorMessage = 'Failed to save birth date.';
    } finally {
      this.saving = false;
    }
  }

  async goHome(): Promise<void> {
    if (this.leaving()) return;
    this.leaving.set(true);
    await new Promise((resolve) => setTimeout(resolve, 250));
    await this.router.navigate(['/']);
  }

  async logout(): Promise<void> {
    if (this.leaving()) return;
    this.leaving.set(true);
    await new Promise((resolve) => setTimeout(resolve, 250));
    await this.router.navigate(['/logout']);
  }
}
