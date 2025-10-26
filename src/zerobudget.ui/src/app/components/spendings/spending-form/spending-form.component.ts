import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { SpendingService } from '../../../services/spending.service';
import { BucketService } from '../../../services/bucket.service';
import { Bucket } from '../../../models/bucket.model';
import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { MatChipInputEvent } from '@angular/material/chips';

@Component({
  selector: 'app-spending-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatChipsModule,
    MatIconModule,
    MatSnackBarModule
  ],
  templateUrl: './spending-form.component.html',
  styleUrl: './spending-form.component.scss'
})
export class SpendingFormComponent implements OnInit {
  spendingForm: FormGroup;
  buckets: Bucket[] = [];
  tags: string[] = [];
  loading = false;
  readonly separatorKeysCodes = [ENTER, COMMA] as const;

  constructor(
    private fb: FormBuilder,
    private spendingService: SpendingService,
    private bucketService: BucketService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.spendingForm = this.fb.group({
      bucketId: ['', [Validators.required]],
      date: [new Date(), [Validators.required]],
      description: ['', [Validators.required]],
      amount: [0, [Validators.required, Validators.min(0.01)]],
      owner: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.loadBuckets();
  }

  loadBuckets(): void {
    this.bucketService.getByName().subscribe({
      next: (buckets) => {
        this.buckets = buckets.filter(b => b.enabled);
      },
      error: (error) => {
        this.snackBar.open('Failed to load buckets', 'Close', { duration: 3000 });
      }
    });
  }

  addTag(event: MatChipInputEvent): void {
    const value = (event.value || '').trim();
    if (value) {
      this.tags.push(value);
    }
    event.chipInput!.clear();
  }

  removeTag(tag: string): void {
    const index = this.tags.indexOf(tag);
    if (index >= 0) {
      this.tags.splice(index, 1);
    }
  }

  onSubmit(): void {
    if (this.spendingForm.valid) {
      this.loading = true;
      const formValue = this.spendingForm.value;
      const date = new Date(formValue.date);
      const dateString = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;

      const command = {
        bucketId: formValue.bucketId,
        date: dateString,
        description: formValue.description,
        amount: formValue.amount,
        owner: formValue.owner,
        tagNames: this.tags
      };

      this.spendingService.create(command).subscribe({
        next: (result) => {
          this.snackBar.open('Spending created successfully', 'Close', { duration: 3000 });
          this.router.navigate(['/spendings']);
        },
        error: (error) => {
          this.loading = false;
          this.snackBar.open('Failed to create spending', 'Close', { duration: 3000 });
        }
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/spendings']);
  }
}
