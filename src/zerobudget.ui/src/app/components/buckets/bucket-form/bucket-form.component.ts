import { Component, Inject, OnInit, Optional } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { BucketService } from '../../../services/bucket.service';
import { Bucket } from '../../../models/bucket.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-bucket-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule,
    MatCardModule
  ],
  templateUrl: './bucket-form.component.html',
  styleUrl: './bucket-form.component.scss'
})
export class BucketFormComponent implements OnInit {
  bucketForm: FormGroup;
  loading = false;
  isEdit = false;
  isStandalone = false; // Flag per distinguere modalità dialog da pagina
  pageTitle = 'Create Bucket';

  constructor(
    private fb: FormBuilder,
    private bucketService: BucketService,
    private snackBar: MatSnackBar,
    private router: Router,
    @Optional() public dialogRef: MatDialogRef<BucketFormComponent> | null = null,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: { bucket: Bucket | null } | null = null
  ) {
    this.isEdit = !!data?.bucket;
    this.isStandalone = !dialogRef; // Se non c'è dialogRef, siamo in modalità standalone
    this.pageTitle = this.isEdit ? 'Edit Bucket' : 'Create Bucket';

    this.bucketForm = this.fb.group({
      name: [data?.bucket?.name || '', [Validators.required]],
      description: [data?.bucket?.description || '', [Validators.required]],
      defaultLimit: [data?.bucket?.defaultLimit || 0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {}

  onSubmit(): void {
    if (this.bucketForm.valid) {
      this.loading = true;
      const formValue = this.bucketForm.value;

      if (this.isEdit && this.data?.bucket) {
        this.bucketService.update(this.data.bucket.id, formValue).subscribe({
          next: (result) => {
            this.snackBar.open('Bucket updated successfully', 'Close', { duration: 3000 });
            if (this.dialogRef) {
              this.dialogRef.close(true);
            } else {
              this.router.navigate(['/buckets']);
            }
          },
          error: (error) => {
            this.loading = false;
            this.snackBar.open('Failed to update bucket', 'Close', { duration: 3000 });
          }
        });
      } else {
        this.bucketService.create(formValue).subscribe({
          next: (result) => {
            this.snackBar.open('Bucket created successfully', 'Close', { duration: 3000 });
            if (this.dialogRef) {
              this.dialogRef.close(true);
            } else {
              this.router.navigate(['/buckets']);
            }
          },
          error: (error) => {
            this.loading = false;
            this.snackBar.open('Failed to create bucket', 'Close', { duration: 3000 });
          }
        });
      }
    }
  }

  onCancel(): void {
    if (this.dialogRef) {
      this.dialogRef.close(false);
    } else {
      this.router.navigate(['/buckets']);
    }
  }
}

