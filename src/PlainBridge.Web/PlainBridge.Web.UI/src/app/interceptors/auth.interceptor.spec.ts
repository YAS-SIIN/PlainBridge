import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http';
import { AuthInterceptor } from './auth.interceptor';

import { environment } from '../../environments/environment';

describe('AuthInterceptor', () => {
  let httpMock: HttpTestingController;
  let http: HttpClient;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
      ]
    });

    httpMock = TestBed.inject(HttpTestingController);
    http = TestBed.inject(HttpClient);
  });

  afterEach(() => {
    httpMock.verify();
    sessionStorage.clear();
  });

  it('adds Authorization header when token present', () => {
    sessionStorage.setItem('auth_token', 'xyz');
    http.get(`${environment.apiUrl}/_ping`).subscribe();
    const req = httpMock.expectOne(`${environment.apiUrl}/_ping`);
    expect(req.request.headers.get('Authorization')).toBe('Bearer xyz');
    req.flush({ ok: true });
  });

  it('does not add Authorization when token missing', () => {
    http.get(`${environment.apiUrl}/_ping`).subscribe();
    const req = httpMock.expectOne(`${environment.apiUrl}/_ping`);
    expect(req.request.headers.has('Authorization')).toBeFalse();
    req.flush({ ok: true });
  });
});

