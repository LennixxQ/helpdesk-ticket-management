import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDividerModule } from '@angular/material/divider';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AuthService } from '../../../core/services/auth.service';
import { MfaService, MfaSetupResponse } from '../../../core/services/mfa.service';
import { ConfirmationDialogComponent } from '../../../shared/components/confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'app-security',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDividerModule,
    MatDialogModule,
    MatTooltipModule,
  ],
  templateUrl: './security.html',
  styleUrl: './security.scss'
})
export class SecurityComponent implements OnInit {
  auth = inject(AuthService);
  private mfaService = inject(MfaService);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);

  // MFA State
  isMfaEnabled = signal(false);
  isLoadingMfa = signal(false);
  showMfaSetup = signal(false);
  showDisableForm = signal(false);
  mfaSetupData = signal<MfaSetupResponse | null>(null);

  // Code input
  verifyCodeControl = new FormControl('', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]);
  disableCodeControl = new FormControl('', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]);

  // MFA Setup form
  setupForm = new FormGroup({
    code: new FormControl('', [Validators.required, Validators.minLength(6), Validators.maxLength(6)])
  });

  get setupCodeControl(): FormControl {
    return this.setupForm.get('code') as FormControl;
  }

  ngOnInit(): void {
    this.checkMfaStatus();
  }

  checkMfaStatus(): void {
    const user = this.auth.currentUser();
    console.log('Decoded User Token in Security:', user);
    this.isMfaEnabled.set(this.auth.isMfaEnabledSignal());
  }

  openMfaSetup(): void {
    this.isLoadingMfa.set(true);
    this.showMfaSetup.set(true);

    this.mfaService.getSetup().subscribe({
      next: (res) => {
        this.isLoadingMfa.set(false);
        if (res.success) {
          this.mfaSetupData.set(res.data);
        }
      },
      error: () => {
        this.isLoadingMfa.set(false);
        this.showSnack('Failed to load MFA setup', 'error');
        this.showMfaSetup.set(false);
      }
    });
  }

  verifyAndEnableMfa(): void {
    const code = this.setupForm.get('code')?.value;
    if (!code || code.length !== 6) {
      this.showSnack('Please enter a valid 6-digit code', 'error');
      return;
    }

    this.isLoadingMfa.set(true);
    this.mfaService.verifySetup(code).subscribe({
      next: (res) => {
        this.isLoadingMfa.set(false);
        if (res.success) {
          this.isMfaEnabled.set(true);
          this.showMfaSetup.set(false);
          this.setupForm.reset();
          this.mfaSetupData.set(null);
          this.showSnack('MFA enabled successfully! Your account is now more secure.', 'success');
        }
      },
      error: (err) => {
        this.isLoadingMfa.set(false);
        const errMsg = this.getErrorMessage(err);
        this.showSnack(errMsg, 'error');
      }
    });
  }

  openDisableMfaDialog(): void {
    const ref = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: 'Change Authenticator',
        message: 'Enter your new authenticator code to update your two-factor authentication.',
        confirmText: 'Verify & Update',
        cancelText: 'Cancel',
        type: 'info'
      }
    });

    ref.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        // User confirmed, now show code input
        const code = prompt('Enter the 6-digit code from your new authenticator app:');
        if (code && code.length === 6) {
          this.performMfaDisable(code);
        } else if (code) {
          this.showSnack('Please enter a valid 6-digit code', 'error');
        }
      }
    });
  }

  private performMfaDisable(code: string): void {
    this.isLoadingMfa.set(true);
    this.mfaService.disable(code).subscribe({
      next: (res) => {
        this.isLoadingMfa.set(false);
        if (res.success) {
          this.isMfaEnabled.set(false);
          this.showMfaSetup.set(false);
          this.showDisableForm.set(false);
          this.disableCodeControl.reset();
          this.showSnack('Two-Factor Authentication disabled successfully', 'success');
        }
      },
      error: (err) => {
        this.isLoadingMfa.set(false);
        const errMsg = this.getErrorMessage(err);
        this.showSnack(errMsg, 'error');
      }
    });
  }

  confirmDisable(): void {
    const code = this.disableCodeControl.value;
    if (!code || code.length !== 6) {
      this.showSnack('Please enter a valid 6-digit code', 'error');
      return;
    }
    this.performMfaDisable(code);
  }

  closeDisableDialog(): void {
    this.dialog.closeAll();
  }

  cancelSetup(): void {
    this.showMfaSetup.set(false);
    this.mfaSetupData.set(null);
    this.setupForm.reset();
  }

  copyToClipboard(text: string): void {
    navigator.clipboard.writeText(text).then(() => {
      this.showSnack('Copied to clipboard', 'success');
    });
  }

  showSnack(msg: string, type: 'success' | 'error'): void {
    this.snackBar.open(msg, 'Close', {
      duration: 4000,
      panelClass: type === 'success' ? ['snack-success'] : ['snack-error']
    });
  }

  getErrorMessage(err: any): string {
    if (!err) return 'An unexpected error occurred.';
    
    // If err is a string
    if (typeof err === 'string') return err;
    
    // If err.error is a string
    if (err.error && typeof err.error === 'string') return err.error;
    
    // If err.error is an object
    if (err.error && typeof err.error === 'object') {
      const errorObj = err.error;
      
      // Try base response message
      if (errorObj.message && typeof errorObj.message === 'string') {
        return errorObj.message;
      }
      
      // Try ASP.NET model validation errors (ValidationProblemDetails)
      if (errorObj.errors && typeof errorObj.errors === 'object') {
        const validationErrors = errorObj.errors;
        const messages: string[] = [];
        for (const key in validationErrors) {
          if (Array.isArray(validationErrors[key])) {
            messages.push(...validationErrors[key]);
          } else if (typeof validationErrors[key] === 'string') {
            messages.push(validationErrors[key]);
          }
        }
        if (messages.length > 0) {
          return messages.join(' ');
        }
      }
      
      // Try general title or description
      if (errorObj.title && typeof errorObj.title === 'string') {
        return errorObj.title;
      }
    }
    
    // If err.message is a string
    if (err.message && typeof err.message === 'string') {
      return err.message;
    }
    
    return 'Invalid code. Please try again.';
  }
}
