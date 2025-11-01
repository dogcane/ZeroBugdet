import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatCardModule } from '@angular/material/card';
import { BucketService } from '../../../services/bucket.service';
import { Bucket } from '../../../models/bucket.model';
import { BucketFormComponent } from '../bucket-form/bucket-form.component';

@Component({
  selector: 'app-bucket-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatDialogModule,
    MatCardModule
  ],
  templateUrl: './bucket-list.component.html',
  styleUrl: './bucket-list.component.scss'
})
export class BucketListComponent implements OnInit {
  buckets: Bucket[] = [];
  displayedColumns: string[] = ['name', 'description', 'defaultLimit', 'enabled', 'actions'];

  constructor(
    private bucketService: BucketService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadBuckets();
  }

  loadBuckets(): void {
    this.bucketService.getByName().subscribe({
      next: (buckets) => {
        this.buckets = buckets;
      },
      error: (error) => {
        this.snackBar.open('Failed to load buckets', 'Close', { duration: 3000 });
      }
    });
  }

  openCreatePage(): void {
    this.router.navigate(['/buckets/new']);
  }

  openEditDialog(bucket: Bucket): void {
    const dialogRef = this.dialog.open(BucketFormComponent, {
      width: '500px',
      data: { bucket }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadBuckets();
      }
    });
  }

  deleteBucket(bucket: Bucket): void {
    if (confirm(`Are you sure you want to delete bucket "${bucket.name}"?`)) {
      this.bucketService.delete(bucket.id).subscribe({
        next: () => {
          this.snackBar.open('Bucket deleted successfully', 'Close', { duration: 3000 });
          this.loadBuckets();
        },
        error: (error) => {
          this.snackBar.open('Failed to delete bucket', 'Close', { duration: 3000 });
        }
      });
    }
  }

  toggleEnabled(bucket: Bucket): void {
    if (!bucket.enabled) {
      this.bucketService.enable(bucket.id).subscribe({
        next: () => {
          this.snackBar.open('Bucket enabled', 'Close', { duration: 3000 });
          this.loadBuckets();
        },
        error: (error) => {
          this.snackBar.open('Failed to enable bucket', 'Close', { duration: 3000 });
        }
      });
    }
  }
}
