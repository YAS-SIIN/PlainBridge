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
    const token = sessionStorage.getItem('auth_token');
    if (!token) {
      return false;
    }
    return !this.isTokenExpired(token);
  }

  private isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      if (!payload.exp) return false; // If no exp claim, assume not expired
      const now = Math.floor(Date.now() / 1000);
      return payload.exp < now;
    } catch {
      return true; // If token is malformed, treat as expired
    }
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
    window.location.href = 'https://localhost:5001/bff/login';
  }
}
