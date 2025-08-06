import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Location } from '@angular/common';

@Component({
  selector: 'app-sign-in-oidc',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sign-in-oidc.component.html',
  styleUrls: ['./sign-in-oidc.component.css']
})
export class SignInOidcComponent implements OnInit {
  
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private location: Location
  ) { }

  ngOnInit(): void {
    // Get the token from query parameters
    this.route.queryParams.subscribe(params => {
      const token = params['access_token'];
      
      if (token) {
        // Store the token in session storage
        sessionStorage.setItem('auth_token', token);
        
        console.log('Token successfully stored in session storage');
        
        // Clear the token from the address bar for security
        this.location.replaceState('/sign-in-oidc');
        
        // Navigate to main page after a short delay to allow user to see the loading screen
        this.router.navigate(['/']);
        
      } else {
        console.warn('No token found in query parameters');
        // If no token, redirect to main page anyway
        setTimeout(() => {
          this.router.navigate(['/']);
        }, 2000);
      }
    });
  }
}
