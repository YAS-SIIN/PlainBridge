import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { ProfileComponent } from './profile.component';
import { UserService } from '../../../services/user.service';
import { ApiResponseService } from '../../../services/api-response.service';
import { ToastService } from '../../../services/toast.service';

class UserServiceStub {
  getCurrentUser() { return of({ resultCode: 0, data: { id: 1, username: 'u', name: 'n', family: 'f', phoneNumber: '1' } }); }
  updateUser() { return of({ resultCode: 0, data: {} }); }
  changePassword() { return of({ resultCode: 0, data: {} }); }
}
class ApiResponseServiceStub {
  handleResponse(obs: any) { return obs.pipe(); }
}
class ToastStub { success(){} error(){} }

describe('ProfileComponent', () => {
  let component: ProfileComponent;
  let fixture: ComponentFixture<ProfileComponent>;
  let userSvc: UserServiceStub;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule],
      declarations: [ProfileComponent],
      providers: [
        { provide: UserService, useClass: UserServiceStub },
        { provide: ApiResponseService, useClass: ApiResponseServiceStub },
        { provide: ToastService, useClass: ToastStub }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(ProfileComponent);
    component = fixture.componentInstance;
    userSvc = TestBed.inject(UserService) as any;
    fixture.detectChanges();
  });

  it('loads current user into profile form', () => {
    expect(component.loading).toBeFalse();
    expect(component.profileForm.value.name).toBe('n');
    expect(component.profileForm.value.family).toBe('f');
    expect(component.profileForm.get('username')?.disabled).toBeTrue();
  });

  it('validates password complexity and matching', () => {
    component.passwordForm.get('currentPassword')?.setValue('oldPass1!');
    component.passwordForm.get('newPassword')?.setValue('Weak');
    component.passwordForm.get('confirmNewPassword')?.setValue('Weak');
    expect(component.passwordForm.invalid).toBeTrue();

    component.passwordForm.get('newPassword')?.setValue('Strong1!');
    component.passwordForm.get('confirmNewPassword')?.setValue('Strong1!');
    expect(component.passwordForm.valid).toBeTrue();
  });

  it('changePassword passes dto and resets on success', () => {
    spyOn(userSvc, 'changePassword').and.returnValue(of({ resultCode: 0, data: {} } as any));
    component.user = { id: 1 } as any;
    component.passwordForm.setValue({ currentPassword: 'aA1!', newPassword: 'StrongPass1!', confirmNewPassword: 'StrongPass1!' });
    component.changePassword();
    expect(userSvc.changePassword).toHaveBeenCalled();
    expect(component.changingPassword).toBeFalse();
  });

  it('applies API errors to password form controls', () => {
    const apiError = { errors: { newPassword: ['too weak'] } };
    spyOn(userSvc, 'changePassword').and.returnValue(throwError(() => apiError));
    component.user = { id: 1 } as any;
    component.passwordForm.setValue({ currentPassword: 'aA1!', newPassword: 'StrongPass1!', confirmNewPassword: 'StrongPass1!' });
    component.changePassword();
    expect(component.passwordForm.get('newPassword')?.errors?.['api']).toBe('too weak');
  });
});

