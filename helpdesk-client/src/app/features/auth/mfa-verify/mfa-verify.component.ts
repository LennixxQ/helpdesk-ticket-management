import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { MfaService } from '../../../core/services/mfa.service';
import { StorageService } from '../../../core/services/storage.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-mfa-verify',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatIconModule,
    MatSnackBarModule,
  ],
  templateUrl: './mfa-verify.html',
  styleUrl: './mfa-verify.scss'
})
export class MfaVerifyComponent implements OnInit {
  private mfaService = inject(MfaService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  private storageService = inject(StorageService);
  private authService = inject(AuthService);

  isLoading = signal(false);
  codeControl = new FormControl('', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]);

  private mfaToken: string | null = null;

  ngOnInit(): void {
    // Get MFA token from query params
    this.route.queryParams.subscribe(params => {
      this.mfaToken = params['token'] || null;
      if (!this.mfaToken) {
        this.snackBar.open('Invalid MFA session. Please login again.', 'Close', {
          duration: 4000,
          panelClass: ['snack-error']
        });
        this.router.navigate(['/login']);
      }
    });
  }

  verify(): void {
    const code = this.codeControl.value;
    if (!code || code.length !== 6) {
      this.snackBar.open('Please enter a valid 6-digit code', 'Close', {
        duration: 3000,
        panelClass: ['snack-error']
      });
      return;
    }

    if (!this.mfaToken) {
      this.snackBar.open('Invalid session. Please login again.', 'Close', {
        duration: 3000,
        panelClass: ['snack-error']
      });
      return;
    }

    this.isLoading.set(true);
    this.mfaService.verifyLogin(code, this.mfaToken).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        if (res.success && res.data?.token) {
          // Store the token and update the auth service
          this.storageService.setToken(res.data.token);
          this.authService.getTokenSignal().set(res.data.token);

          this.snackBar.open('Verification successful!', 'Close', {
            duration: 3000,
            panelClass: ['snack-success']
          });
          this.router.navigate(['/dashboard']);
        } else {
          this.snackBar.open(res.message || 'Invalid code. Please try again.', 'Close', {
            duration: 4000,
            panelClass: ['snack-error']
          });
        }
      },
      error: (err) => {
        this.isLoading.set(false);
        this.snackBar.open(err?.error?.message || 'Verification failed. Please try again.', 'Close', {
          duration: 4000,
          panelClass: ['snack-error']
        });
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/login']);
  }
}