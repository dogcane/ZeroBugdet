import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MonthlyService } from '../../../services/monthly.service';
import { MonthlyBucket, MonthlySpending } from '../../../models/monthly.model';

@Component({
  selector: 'app-monthly-view',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatSelectModule,
    MatTableModule,
    MatButtonModule,
    MatSnackBarModule
  ],
  templateUrl: './monthly-view.component.html',
  styleUrl: './monthly-view.component.scss'
})
export class MonthlyViewComponent implements OnInit {
  currentYear = new Date().getFullYear();
  currentMonth = new Date().getMonth() + 1;
  selectedYear = this.currentYear;
  selectedMonth = this.currentMonth;
  
  years: number[] = [];
  months = [
    { value: 1, name: 'January' },
    { value: 2, name: 'February' },
    { value: 3, name: 'March' },
    { value: 4, name: 'April' },
    { value: 5, name: 'May' },
    { value: 6, name: 'June' },
    { value: 7, name: 'July' },
    { value: 8, name: 'August' },
    { value: 9, name: 'September' },
    { value: 10, name: 'October' },
    { value: 11, name: 'November' },
    { value: 12, name: 'December' }
  ];

  monthlyBuckets: MonthlyBucket[] = [];
  monthlySpendings: MonthlySpending[] = [];
  
  bucketColumns: string[] = ['bucketName', 'limit', 'total', 'remaining'];
  spendingColumns: string[] = ['date', 'description', 'amount', 'owner'];

  constructor(
    private monthlyService: MonthlyService,
    private snackBar: MatSnackBar
  ) {
    // Generate years from 3 years ago to 2 years in the future
    for (let i = -3; i <= 2; i++) {
      this.years.push(this.currentYear + i);
    }
  }

  ngOnInit(): void {
    this.loadMonthlyData();
  }

  loadMonthlyData(): void {
    this.monthlyService.getMonthlyBucketsByYearMonth(this.selectedYear, this.selectedMonth).subscribe({
      next: (buckets) => {
        this.monthlyBuckets = buckets;
      },
      error: (error) => {
        this.snackBar.open('Failed to load monthly buckets', 'Close', { duration: 3000 });
      }
    });

    const startDate = `${this.selectedYear}-${String(this.selectedMonth).padStart(2, '0')}-01`;
    const lastDay = new Date(this.selectedYear, this.selectedMonth, 0).getDate();
    const endDate = `${this.selectedYear}-${String(this.selectedMonth).padStart(2, '0')}-${lastDay}`;

    this.monthlyService.getMonthlySpendingsByDateRange(startDate, endDate).subscribe({
      next: (spendings) => {
        this.monthlySpendings = spendings;
      },
      error: (error) => {
        this.snackBar.open('Failed to load monthly spendings', 'Close', { duration: 3000 });
      }
    });
  }

  onMonthYearChange(): void {
    this.loadMonthlyData();
  }

  generateMonthlyData(): void {
    this.monthlyService.generateMonthlyData({ year: this.selectedYear, month: this.selectedMonth }).subscribe({
      next: () => {
        this.snackBar.open('Monthly data generated successfully', 'Close', { duration: 3000 });
        this.loadMonthlyData();
      },
      error: (error) => {
        this.snackBar.open('Failed to generate monthly data', 'Close', { duration: 3000 });
      }
    });
  }

  getRemainingAmount(bucket: MonthlyBucket): number {
    return bucket.limit - bucket.total;
  }
}
