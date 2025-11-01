import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MonthlyBucket, MonthlySpending, GenerateMonthlyDataRequest } from '../models/monthly.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MonthlyService {
  private bucketApiUrl = `${environment.apiUrl}/monthlybucket`;
  private spendingApiUrl = `${environment.apiUrl}/monthlyspending`;

  constructor(private http: HttpClient) { }

  // ===== Monthly Buckets =====

  getMonthlyBucketById(id: number): Observable<MonthlyBucket> {
    return this.http.get<MonthlyBucket>(`${this.bucketApiUrl}/${id}`);
  }

  /**
   * Get monthly buckets with optional filters for year, month, bucketId, and description
   * Uses GetMonthlyBucketsQuery handler with optional parameters
   */
  getAllMonthlyBuckets(year?: number, month?: number, bucketId?: number, description?: string): Observable<MonthlyBucket[]> {
    let params = new HttpParams();

    if (year !== undefined) {
      params = params.set('year', year.toString());
    }
    if (month !== undefined) {
      params = params.set('month', month.toString());
    }
    if (bucketId !== undefined) {
      params = params.set('bucketId', bucketId.toString());
    }
    if (description !== undefined && description.length > 0) {
      params = params.set('description', description);
    }

    return this.http.get<MonthlyBucket[]>(this.bucketApiUrl, { params });
  }

  generateMonthlyData(request: GenerateMonthlyDataRequest): Observable<void> {
    return this.http.post<void>(`${this.bucketApiUrl}/generate`, request);
  }

  // ===== Monthly Spendings =====

  getMonthlySpendingById(id: number): Observable<MonthlySpending> {
    return this.http.get<MonthlySpending>(`${this.spendingApiUrl}/${id}`);
  }

  /**
   * Get monthly spendings with optional filters for monthlyBucketId, description, owner, and date range
   * Uses GetMonthlySpendingsQuery handler with optional parameters
   */
  getAllMonthlySpendings(monthlyBucketId?: number, description?: string, owner?: string, startDate?: DateOnly, endDate?: DateOnly): Observable<MonthlySpending[]> {
    let params = new HttpParams();

    if (monthlyBucketId !== undefined) {
      params = params.set('monthlyBucketId', monthlyBucketId.toString());
    }
    if (description !== undefined && description.length > 0) {
      params = params.set('description', description);
    }
    if (owner !== undefined && owner.length > 0) {
      params = params.set('owner', owner);
    }
    if (startDate !== undefined) {
      params = params.set('startDate', startDate.toString());
    }
    if (endDate !== undefined) {
      params = params.set('endDate', endDate.toString());
    }

    return this.http.get<MonthlySpending[]>(this.spendingApiUrl, { params });
  }
}

// Type alias for DateOnly - represents a date without time component
type DateOnly = string; // Format: YYYY-MM-DD

