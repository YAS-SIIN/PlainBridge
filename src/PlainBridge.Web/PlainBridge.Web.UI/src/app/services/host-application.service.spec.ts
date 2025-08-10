import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { environment } from '../../environments/environment';
import { HostApplicationService } from './host-application.service';
import { AuthInterceptor, AuthInterceptorProvider } from '../interceptors/auth.interceptor';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

describe('HostApplicationService', () => {
  let service: HostApplicationService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        HostApplicationService,
        { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
      ]
    });

    service = TestBed.inject(HostApplicationService);
    httpMock = TestBed.inject(HttpTestingController);
    sessionStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should GET list', () => {
    const url = `${environment.apiUrl}/HostApplication`;
    service.getAllApplications().subscribe();
    const req = httpMock.expectOne(url);
    expect(req.request.method).toBe('GET');
    req.flush({ resultCode: 0, data: [] });
  });

  it('should GET by id', () => {
    const id = 10;
    const url = `${environment.apiUrl}/HostApplication/${id}`;
    service.getApplication(id).subscribe();
    const req = httpMock.expectOne(url);
    expect(req.request.method).toBe('GET');
    req.flush({ resultCode: 0, data: {} });
  });

  it('should POST create with body', () => {
    const url = `${environment.apiUrl}/HostApplication`;
    const body = { name: 'A' } as any;
    service.createApplication(body).subscribe();
    const req = httpMock.expectOne(url);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(body);
    req.flush({ resultCode: 0, data: 'ok' });
  });

  it('should PATCH update with body', () => {
    const id = 3;
    const url = `${environment.apiUrl}/HostApplication/${id}`;
    const body = { name: 'B' } as any;
    service.updateApplication(id, body).subscribe();
    const req = httpMock.expectOne(url);
    expect(req.request.method).toBe('PATCH');
    expect(req.request.body).toEqual(body);
    req.flush({ resultCode: 0, data: {} });
  });

  it('should PATCH UpdateState with empty body', () => {
    const id = 5;
    const isActive = true;
    const url = `${environment.apiUrl}/HostApplication/UpdateState/${id}/${isActive}`;
    service.patchIsActive(id, isActive).subscribe();
    const req = httpMock.expectOne(url);
    expect(req.request.method).toBe('PATCH');
    expect(req.request.body).toEqual({});
    req.flush({ resultCode: 0, data: {} });
  });

  it('should send Authorization header when token exists', () => {
    sessionStorage.setItem('auth_token', 'abc');
    service.getAllApplications().subscribe();
    const req = httpMock.expectOne(`${environment.apiUrl}/HostApplication`);
    expect(req.request.headers.get('Authorization')).toMatch(/^Bearer\s/);
    req.flush({ resultCode: 0, data: [] });
  });
});

