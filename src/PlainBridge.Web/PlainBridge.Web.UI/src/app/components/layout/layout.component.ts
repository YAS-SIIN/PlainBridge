import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';
import { UserDto } from '../../models';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-layout',
  standalone: false,
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css']
})
export class LayoutComponent implements OnInit {
  currentUser: UserDto | null = null;
  isAuthenticated$: Observable<boolean>;

  constructor(
    private authService: AuthService,
    private userService: UserService
  ) {
    this.isAuthenticated$ = this.authService.isAuthenticated$;
  }

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.loadCurrentUser();
    }
  }

  private loadCurrentUser(): void {
    this.userService.getCurrentUser().subscribe({
      next: (result) => {
        if (result.resultCode === 0) {
          this.currentUser = result.data;
        } else {
          this.authService.logout();
        }
      },
      error: (error) => {
        console.error('Error loading current user:', error);
        this.authService.logout();
      }
    });
  }

  logout(): void {
    this.authService.logout();
  }

  login(): void {
    this.authService.login();
  }
}
