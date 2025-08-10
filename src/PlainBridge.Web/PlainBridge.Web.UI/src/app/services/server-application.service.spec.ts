import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { environment } from '../../environments/environment';
import { ServerApplicationService } from './server-application.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from '../interceptors/auth.interceptor';

describe('ServerApplicationService', () => {
  let service: ServerApplicationService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        ServerApplicationService,
        { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
      ]
    });

    service = TestBed.inject(ServerApplicationService);
    httpMock = TestBed.inject(HttpTestingController);
    sessionStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should GET list', () => {
    const url = `${environment.apiUrl}/ServerApplication`;
    service.getAllApplications().subscribe();
    const req = httpMock.expectOne(url);
    expect(req.request.method).toBe('GET');
    req.flush({ resultCode: 0, data: [] });
  });

  it('should GET by id', () => {
    const id = 10;
    const url = `${environment.apiUrl}/ServerApplication/${id}`;
    service.getApplication(id).subscribe();
    const req = httpMock.expectOne(url);
    expect(req.request.method).toBe('GET');
    req.flush({ resultCode: 0, data: {} });
  });

  it('should POST create with body', () => {
    const url = `${environment.apiUrl}/ServerApplication`;
    const body = { name: 'A' } as any;
    service.createApplication(body).subscribe();
    const req = httpMock.expectOne(url);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(body);
    req.flush({ resultCode: 0, data: 'ok' });
  });

  it('should PATCH update with body', () => {
    const id = 3;
    const url = `${environment.apiUrl}/ServerApplication/${id}`;
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
    const url = `${environment.apiUrl}/ServerApplication/UpdateState/${id}/${isActive}`;
    service.patchIsActive(id, isActive).subscribe();
    const req = httpMock.expectOne(url);
    expect(req.request.method).toBe('PATCH');
    expect(req.request.body).toEqual({});
    req.flush({ resultCode: 0, data: {} });
  });

  it('should send Authorization header when token exists', () => {
    sessionStorage.setItem('auth_token', 'abc');
    service.getAllApplications().subscribe();
    const req = httpMock.expectOne(`${environment.apiUrl}/ServerApplication`);
    expect(req.request.headers.get('Authorization')).toMatch(/^Bearer\s/);
    req.flush({ resultCode: 0, data: [] });
  });
});

