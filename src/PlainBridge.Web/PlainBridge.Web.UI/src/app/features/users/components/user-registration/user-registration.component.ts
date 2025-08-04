import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../../../../services/user.service';
import { ToastService } from '../../../../services/toast.service';
import { Router } from '@angular/router';
import { CreateUserDto } from '../../../../models';

@Component({
  selector: 'app-user-registration',
  standalone: false,
  templateUrl: './user-registration.component.html',
  styleUrls: ['./user-registration.component.css']
})
export class UserRegistrationComponent implements OnInit {
  registrationForm: FormGroup;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private toastService: ToastService,
    private router: Router
  ) {
    this.registrationForm = this.fb.group({
      username: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
      name: ['', [Validators.required]],
      family: ['', [Validators.required]],
      phoneNumber: ['']
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit(): void {}

  onSubmit(): void {
    if (this.registrationForm.invalid) {
      return;
    }
    this.loading = true;
    const user = this.registrationForm.value;

    this.userService.createUser(user).subscribe({
      next: () => {
        this.toastService.success('User registered successfully');
        this.router.navigate(['/users']);
        this.loading = false;
      },
      error: () => {
        this.toastService.error('Error registering user');
        this.loading = false;
      }
    });
  }

  private passwordMatchValidator(form: FormGroup): null | { mismatch: true } {
    return form.get('password')?.value === form.get('confirmPassword')?.value ? null : { mismatch: true };
  }
}
