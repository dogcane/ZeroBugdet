import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-initial-check',
  standalone: true,
  template: '<div style="display: flex; justify-content: center; align-items: center; height: 100vh;"><h2>Loading...</h2></div>'
})
export class InitialCheckComponent implements OnInit {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authService.isMainUserRequired().subscribe({
      next: (response) => {
        if (response.isRequired) {
          this.router.navigate(['/register-main-user']);
        } else {
          this.router.navigate(['/login']);
        }
      },
      error: (error) => {
        console.error('Error checking main user:', error);
        this.router.navigate(['/login']);
      }
    });
  }
}
