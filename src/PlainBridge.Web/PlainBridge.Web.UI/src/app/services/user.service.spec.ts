import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { UserService } from './user.service';
import { environment } from '../../environments/environment';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from '../interceptors/auth.interceptor';

describe('UserService', () => {
  let service: UserService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        UserService,
        { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
      ]
    });

    service = TestBed.inject(UserService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('GET /User/GetCurrentUser', () => {
    service.getCurrentUser().subscribe();
    const req = httpMock.expectOne(`${environment.apiUrl}/User/GetCurrentUser`);
    expect(req.request.method).toBe('GET');
    req.flush({ resultCode: 0, data: {} });
  });

  it('PATCH /User/ChangePassword with dto', () => {
    const dto = { id: 1, currentPassword: 'a', newPassword: 'b', confirmPassword: 'b' };
    service.changePassword(dto as any).subscribe();
    const req = httpMock.expectOne(`${environment.apiUrl}/User/ChangePassword`);
    expect(req.request.method).toBe('PATCH');
    expect(req.request.body).toEqual(dto);
    req.flush({ resultCode: 0, data: {} });
  });
});

