import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { UserService } from '../../../services/user.service';
import { ApiResponseService } from '../../../services/api-response.service';
import { ToastService } from '../../../services/toast.service';
import { UserDto, UserProfileDto, ChangeUserPasswordDto } from '../../../models';
import { AuthService } from '../../../services';

@Component({
  selector: 'app-profile',
  standalone: false,
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  loading = true;
  savingProfile = false;
  changingPassword = false;

  user: UserDto | null = null;

  profileForm!: FormGroup;
  passwordForm!: FormGroup;

  hideCurrent = true;
  hideNew = true;
  hideConfirm = true;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private apiResponseService: ApiResponseService,
    private toast: ToastService
  ) {}

  ngOnInit(): void {
    debugger
    this.profileForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      family: ['', [Validators.required, Validators.maxLength(100)]],
      username: [{ value: '', disabled: true }, [Validators.required]],
      phoneNumber: ['']
    });

    this.passwordForm = this.fb.group({
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.minLength(8), this.passwordComplexityValidator]],
      confirmNewPassword: ['', [Validators.required]]
    }, { validators: [this.matchPasswordsValidator, this.newNotEqualCurrentValidator] });

    this.loadProfile();

    // Re-validate when fields change for strength/match feedback
    this.passwordForm.get('newPassword')!.valueChanges.subscribe(() => this.passwordForm.updateValueAndValidity({ onlySelf: false }));
    this.passwordForm.get('currentPassword')!.valueChanges.subscribe(() => this.passwordForm.updateValueAndValidity({ onlySelf: false }));
    this.passwordForm.get('confirmNewPassword')!.valueChanges.subscribe(() => this.passwordForm.updateValueAndValidity({ onlySelf: false }));
  }

  private loadProfile(): void {
    this.loading = true;
    this.userService.getCurrentUser().subscribe({
      next: (result) => {
        this.loading = false;
        if (result.resultCode === 0) {
          this.user = result.data;
          this.profileForm.patchValue({
            name: this.user?.name || '',
            family: this.user?.family || '',
            username: this.user?.username || '',
            phoneNumber: this.user?.phoneNumber || ''
          });
        } else {
          this.toast.error(result.resultDescription || 'Failed to load profile');
        }
      },
      error: () => { 
        this.loading = false;
        this.toast.error('Failed to load profile');
      }
    });
  }

  saveProfile(): void {
    if (!this.user) return;
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    const dto: UserDto = {
      ...this.user,
      name: this.profileForm.value.name,
      family: this.profileForm.value.family,
      phoneNumber: this.profileForm.value.phoneNumber
    } as UserDto;

    this.savingProfile = true;
    this.apiResponseService.handleResponse(this.userService.updateUser(this.user.id, dto), { successMessage: 'Profile updated' }).subscribe({
      next: () => { this.savingProfile = false; },
      error: () => { this.savingProfile = false; }
    });
  }

  changePassword(): void {
    if (!this.user) return;
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }

    const dto: ChangeUserPasswordDto = {
      id: this.user.id,
      currentPassword: this.passwordForm.value.currentPassword,
      newPassword: this.passwordForm.value.newPassword,
      confirmPassword: this.passwordForm.value.confirmNewPassword
    };

    this.changingPassword = true;
    this.apiResponseService.handleResponse(this.userService.changePassword(dto), { successMessage: 'Password changed successfully' }).subscribe({
      next: () => {
        this.changingPassword = false;
        this.passwordForm.reset();
      },
      error: (err) => {
        this.changingPassword = false;
        if (err && err.errors) {
          this.applyErrorsToPasswordForm(err.errors);
        }
      }
    });
  }

  private applyErrorsToPasswordForm(errors: { [key: string]: string[] }): void {
    Object.keys(errors || {}).forEach(key => {
      const control = this.passwordForm.get(key);
      if (control) {
        control.setErrors({ api: errors[key][0] || 'Invalid value' });
      }
    });
  }

  passwordComplexityValidator(control: AbstractControl): ValidationErrors | null {
    const value: string = control.value || '';
    if (!value) return null;
    const hasUpper = /[A-Z]/.test(value);
    const hasLower = /[a-z]/.test(value);
    const hasNumber = /\d/.test(value);
    const hasSymbol = /[^A-Za-z0-9]/.test(value);
    return hasUpper && hasLower && hasNumber && hasSymbol ? null : { complexity: true };
  }

  matchPasswordsValidator = (group: AbstractControl): ValidationErrors | null => {
    const newPass = group.get('newPassword')?.value;
    const confirm = group.get('confirmNewPassword')?.value;
    return newPass && confirm && newPass !== confirm ? { mismatch: true } : null;
  };

  newNotEqualCurrentValidator = (group: AbstractControl): ValidationErrors | null => {
    const current = group.get('currentPassword')?.value;
    const next = group.get('newPassword')?.value;
    return current && next && current === next ? { sameAsCurrent: true } : null;
  };

  get strength(): number {
    const v: string = this.passwordForm.get('newPassword')?.value || '';
    let s = 0;
    if (v.length >= 8) s++;
    if (/[A-Z]/.test(v)) s++;
    if (/[a-z]/.test(v)) s++;
    if (/\d/.test(v)) s++;
    if (/[^A-Za-z0-9]/.test(v)) s++;
    return Math.min(s, 4);
  }

  get f() { return this.profileForm.controls; }
  get p() { return this.passwordForm.controls; }
}
