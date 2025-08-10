import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AuthGuard } from './auth.guard';

class AuthServiceStub {
  private authed = false;
  isAuthenticated() { return this.authed; }
  login = jasmine.createSpy('login');
  set(val: boolean) { this.authed = val; }
}

describe('AuthGuard', () => {
  let guard: AuthGuard;
  let auth: AuthServiceStub;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      providers: [
        AuthGuard,
        { provide: (window as any).AuthService || 'AuthService', useClass: AuthServiceStub }
      ]
    });
    // We cannot inject by string token; instead manually construct guard with stub
    auth = new AuthServiceStub();
    guard = new AuthGuard(auth as any, {} as any);
  });

  it('allows navigation when authenticated', () => {
    auth.set(true);
    expect(guard.canActivate()).toBeTrue();
  });

  it('redirects to login when not authenticated', () => {
    auth.set(false);
    expect(guard.canActivate()).toBeFalse();
    expect(auth.login).toHaveBeenCalled();
  });
});

