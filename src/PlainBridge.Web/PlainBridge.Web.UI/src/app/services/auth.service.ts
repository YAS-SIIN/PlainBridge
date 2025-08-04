import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(private router: Router) {}

  private hasToken(): boolean {
    return !!sessionStorage.getItem('auth_token');
  }

  public getToken(): string | null {
    return sessionStorage.getItem('auth_token');
  }

  public setToken(token: string): void {
    sessionStorage.setItem('auth_token', token);
    this.isAuthenticatedSubject.next(true);
  }

  public removeToken(): void {
    sessionStorage.removeItem('auth_token');
    this.isAuthenticatedSubject.next(false);
  }

  public logout(): void {
    this.removeToken();
    this.router.navigate(['/login']);
  }

  public isAuthenticated(): boolean {
    return this.hasToken();
  }

  public login(): void {
    // Redirect to IdentityServer login
    window.location.href = '/bff/login';
  }
}
