import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AbstractControl, NonNullableFormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', Validators.required]
  }, { validators: Register.passwordsMatch });

  serverErrors: string[] = [];
  submitting = false;

  static passwordsMatch(c: AbstractControl): ValidationErrors | null {
    return c.get('password')!.value === c.get('confirmPassword')!.value
      ? null
      : { passwordsMismatch: true };
  }

  async submit(): Promise<void> {
    this.serverErrors = [];
    if (this.form.invalid || this.submitting) return;
    this.submitting = true;
    try {
      const { email, password } = this.form.value;
      const result = await this.auth.register(email!, password!);
      if (result.errors.length === 0) {
        await this.router.navigate(['/login']);
      } else {
        this.serverErrors = result.errors;
      }
    } finally {
      this.submitting = false;
    }
  }
}