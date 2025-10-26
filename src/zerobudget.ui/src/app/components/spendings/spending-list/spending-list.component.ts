import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { SpendingService } from '../../../services/spending.service';
import { Spending } from '../../../models/spending.model';

@Component({
  selector: 'app-spending-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatCardModule
  ],
  templateUrl: './spending-list.component.html',
  styleUrl: './spending-list.component.scss'
})
export class SpendingListComponent implements OnInit {
  spendings: Spending[] = [];
  displayedColumns: string[] = ['date', 'description', 'amount', 'owner', 'tags', 'actions'];

  constructor(
    private spendingService: SpendingService,
    private snackBar: MatSnackBar,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadSpendings();
  }

  loadSpendings(): void {
    this.spendingService.getAll().subscribe({
      next: (spendings) => {
        this.spendings = spendings;
      },
      error: (error) => {
        this.snackBar.open('Failed to load spendings', 'Close', { duration: 3000 });
      }
    });
  }

  deleteSpending(spending: Spending): void {
    if (confirm(`Are you sure you want to delete this spending?`)) {
      this.spendingService.delete(spending.id).subscribe({
        next: () => {
          this.snackBar.open('Spending deleted successfully', 'Close', { duration: 3000 });
          this.loadSpendings();
        },
        error: (error) => {
          this.snackBar.open('Failed to delete spending', 'Close', { duration: 3000 });
        }
      });
    }
  }

  createNew(): void {
    this.router.navigate(['/spendings/new']);
  }
}
