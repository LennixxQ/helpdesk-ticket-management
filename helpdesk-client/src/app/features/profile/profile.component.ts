import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-profile',
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
    MatTooltipModule,
  ],
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class ProfileComponent implements OnInit {
  auth = inject(AuthService);
  private snackBar = inject(MatSnackBar);
  private router = inject(Router);

  isSaving = signal(false);
  activeTab = signal<'profile' | 'security'>('profile');
  posX = signal(50);
  posY = signal(50);

  profileForm = new FormGroup({
    fullName: new FormControl('', [Validators.required]),
    email: new FormControl({ value: '', disabled: true }),
    phone: new FormControl(''),
    department: new FormControl({ value: '', disabled: true }),
    role: new FormControl({ value: '', disabled: true }),
  });

  passwordForm = new FormGroup({
    currentPassword: new FormControl('', [Validators.required]),
    newPassword: new FormControl('', [Validators.required, Validators.minLength(8)]),
    confirmPassword: new FormControl('', [Validators.required]),
  });

  ngOnInit(): void {
    this.loadProfile();
    this.loadPosition();
  }

  loadPosition(): void {
    const pos = this.auth.profilePicPosition().split(' ');
    if (pos.length === 2) {
      this.posX.set(parseInt(pos[0]) || 50);
      this.posY.set(parseInt(pos[1]) || 50);
    }
  }

  loadProfile(): void {
    const user: any = this.auth.currentUser();
    if (user) {
      this.profileForm.patchValue({
        fullName: user['fullName'] || user['name'] || user['unique_name'] || '',
        email: user['email'] || '',
        phone: user['phone'] || '',
        department: user['departmentName'] || 'Not assigned',
        role: this.getRoleLabel(),
      });
    }
  }

  saveProfile(): void {
    if (this.profileForm.invalid) return;
    this.isSaving.set(true);

    // Simulate save - in real app call API
    setTimeout(() => {
      this.isSaving.set(false);
      this.showSnack('Profile updated successfully', 'success');
    }, 800);
  }

  changePassword(): void {
    if (this.passwordForm.invalid) return;

    const newPass = this.passwordForm.get('newPassword')?.value;
    const confirmPass = this.passwordForm.get('confirmPassword')?.value;

    if (newPass !== confirmPass) {
      this.showSnack('Passwords do not match', 'error');
      return;
    }

    this.isSaving.set(true);
    setTimeout(() => {
      this.isSaving.set(false);
      this.passwordForm.reset();
      this.showSnack('Password changed successfully', 'success');
    }, 800);
  }

  getRoleBadgeClass(): string {
    const role = this.auth.currentRole();
    if (!role) return 'role-user';
    const roleNum = typeof role === 'string' ? parseInt(role) : role;
    switch (roleNum) {
      case 1: return 'role-admin';
      case 2: return 'role-agent';
      case 3: return 'role-user';
      case 4: return 'role-departmenthead';
      default: return 'role-user';
    }
  }

  getRoleLabel(): string {
    const role = this.auth.currentRole();
    if (!role) return 'User';
    const roleNum = typeof role === 'string' ? parseInt(role) : role;
    switch (roleNum) {
      case 1: return 'Admin';
      case 2: return 'Agent';
      case 3: return 'User';
      case 4: return 'Department Head';
      default: return 'User';
    }
  }

  isMfaEnabled(): boolean {
    return this.auth.isMfaEnabledSignal();
  }

  showSnack(msg: string, type: 'success' | 'error'): void {
    this.snackBar.open(msg, 'Close', {
      duration: 3000,
      panelClass: type === 'success' ? ['snack-success'] : ['snack-error']
    });
  }

  goToSecurity(): void {
    this.router.navigate(['/profile/security']);
  }

  get profilePicUrl(): string | null {
    return this.auth.profilePic();
  }

  get profilePicPosition(): string {
    return this.auth.profilePicPosition();
  }

  triggerFileInput(inputElement: HTMLInputElement): void {
    inputElement.click();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const file = input.files[0];
    if (!file.type.startsWith('image/')) {
      this.showSnack('Please select a valid image file', 'error');
      return;
    }

    const reader = new FileReader();
    reader.onload = () => {
      const base64String = reader.result as string;
      this.auth.setProfilePic(base64String);
      this.setPreset(50, 50);
      this.showSnack('Profile picture updated successfully', 'success');
    };
    reader.onerror = () => {
      this.showSnack('Error reading image file', 'error');
    };
    reader.readAsDataURL(file);
  }

  onPosXChange(event: Event): void {
    const val = parseInt((event.target as HTMLInputElement).value);
    this.posX.set(val);
    this.auth.setProfilePicPosition(`${this.posX()}% ${this.posY()}%`);
  }

  onPosYChange(event: Event): void {
    const val = parseInt((event.target as HTMLInputElement).value);
    this.posY.set(val);
    this.auth.setProfilePicPosition(`${this.posX()}% ${this.posY()}%`);
  }

  setPreset(x: number, y: number): void {
    this.posX.set(x);
    this.posY.set(y);
    this.auth.setProfilePicPosition(`${x}% ${y}%`);
  }
}

class FormGroup {
  controls: Record<string, FormControl>;
  value: Record<string, any> = {};
  invalid = false;
  constructor(initial: Record<string, any> = {}) {
    this.controls = {};
    for (const [key, value] of Object.entries(initial)) {
      this.controls[key] = new FormControl(value?.value ?? value ?? null);
    }
    this.updateValue();
  }
  get(key: string): FormControl { return this.controls[key]; }
  patchValue(values: Record<string, any>): void {
    for (const [key, value] of Object.entries(values)) {
      if (this.controls[key]) this.controls[key].setValue(value);
    }
    this.updateValue();
  }
  reset(): void {
    for (const key of Object.keys(this.controls)) {
      this.controls[key].setValue(null);
    }
    this.updateValue();
  }
  private updateValue(): void {
    this.value = {};
    for (const [key, control] of Object.entries(this.controls)) {
      this.value[key] = control.value;
    }
  }
}