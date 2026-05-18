import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';
import { StorageService } from '../../../core/services/storage.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private storageService = inject(StorageService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  readonly features = [
    { icon: 'confirmation_number', label: 'Track support tickets in real-time' },
    { icon: 'group', label: 'Collaborate across teams' },
    { icon: 'insights', label: 'Monitor performance with dashboards' },
  ];

  isLoading = signal(false);
  showPassword = signal(false);

  form: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  get emailError(): string {
    const c = this.form.get('email');
    if (c?.hasError('required')) return 'Email is required';
    if (c?.hasError('email')) return 'Enter a valid email address';
    return '';
  }

  get passwordError(): string {
    const c = this.form.get('password');
    if (c?.hasError('required')) return 'Password is required';
    if (c?.hasError('minlength')) return 'Password must be at least 6 characters';
    return '';
  }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    this.isLoading.set(true);

    this.authService.login(this.form.value).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        if (res.success) {
          // Check if MFA is required
          const mfaSessionToken = res.data?.mfaSessionToken;
          if (mfaSessionToken) {
            // Redirect to MFA verification page
            this.router.navigate(['/mfa-verify'], { queryParams: { token: mfaSessionToken } });
          } else {
            // Normal login - proceed to dashboard
            console.log('Login successful');
            this.snackBar.open('Welcome back!', 'Close', {
              duration: 3000,
              panelClass: ['snack-success']
            });
            this.router.navigate(['/dashboard']);
          }
        } else {
          this.snackBar.open(res.message || 'Login failed', 'Close', {
            duration: 4000,
            panelClass: ['snack-error']
          });
        }
      },
      error: (err) => {
        this.isLoading.set(false);
        console.error('Login error:', err);
        this.snackBar.open('Invalid email or password', 'Close', {
          duration: 4000,
          panelClass: ['snack-error']
        });
      }
    });
  }
}