import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });

  errorMessage = '';
  submitting = false;
  showPassword = false;
  shake = false;
  leaving = false;

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  async submit(): Promise<void> {
    this.errorMessage = '';
    if (this.form.invalid || this.submitting || this.leaving) return;
    this.submitting = true;
    try {
      await this.auth.login(this.form.controls.email.value, this.form.controls.password.value);
      this.leaving = true;
      // Let the success motion land before navigating.
      await new Promise((resolve) => setTimeout(resolve, 450));
      await this.router.navigate(['/']);
    } catch {
      this.errorMessage = 'Invalid email or password.';
      this.nudge();
    } finally {
      this.submitting = false;
    }
  }

  private nudge(): void {
    this.shake = false;
    // Re-trigger the shake animation on consecutive failures.
    requestAnimationFrame(() => {
      this.shake = true;
      setTimeout(() => (this.shake = false), 450);
    });
  }
}
