import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../../services/auth.service';
import { User, UserInvitation } from '../../../models/user.model';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSnackBarModule
  ],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss'
})
export class UserListComponent implements OnInit {
  users: User[] = [];
  invitations: UserInvitation[] = [];
  newUserEmail = '';
  
  userColumns: string[] = ['email', 'isMainUser', 'actions'];
  invitationColumns: string[] = ['email', 'expiresAt', 'isUsed', 'actions'];

  constructor(
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadUsers();
    this.loadInvitations();
  }

  loadUsers(): void {
    this.authService.getAllUsers().subscribe({
      next: (users) => {
        this.users = users;
      },
      error: (error) => {
        this.snackBar.open('Failed to load users', 'Close', { duration: 3000 });
      }
    });
  }

  loadInvitations(): void {
    this.authService.getInvitations().subscribe({
      next: (invitations) => {
        this.invitations = invitations;
      },
      error: (error) => {
        this.snackBar.open('Failed to load invitations', 'Close', { duration: 3000 });
      }
    });
  }

  inviteUser(): void {
    if (this.newUserEmail) {
      this.authService.inviteUser({ email: this.newUserEmail }).subscribe({
        next: (response) => {
          this.snackBar.open('User invited successfully', 'Close', { duration: 3000 });
          this.newUserEmail = '';
          this.loadInvitations();
        },
        error: (error) => {
          this.snackBar.open('Failed to invite user', 'Close', { duration: 3000 });
        }
      });
    }
  }

  deleteUser(user: User): void {
    if (user.isMainUser) {
      this.snackBar.open('Cannot delete main user', 'Close', { duration: 3000 });
      return;
    }
    
    if (confirm(`Are you sure you want to delete user ${user.email}?`)) {
      this.authService.deleteUser(user.id).subscribe({
        next: () => {
          this.snackBar.open('User deleted successfully', 'Close', { duration: 3000 });
          this.loadUsers();
        },
        error: (error) => {
          this.snackBar.open('Failed to delete user', 'Close', { duration: 3000 });
        }
      });
    }
  }
}
