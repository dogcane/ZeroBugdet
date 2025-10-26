import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MonthlyBucket, MonthlySpending, GenerateMonthlyDataRequest } from '../models/monthly.model';

@Injectable({
  providedIn: 'root'
})
export class MonthlyService {
  private bucketApiUrl = 'https://localhost:7000/api/monthlybucket'; // TODO: Configure from environment
  private spendingApiUrl = 'https://localhost:7000/api/monthlyspending';

  constructor(private http: HttpClient) {}

  // Monthly Buckets
  getMonthlyBucketById(id: number): Observable<MonthlyBucket> {
    return this.http.get<MonthlyBucket>(`${this.bucketApiUrl}/${id}`);
  }

  getAllMonthlyBuckets(): Observable<MonthlyBucket[]> {
    return this.http.get<MonthlyBucket[]>(this.bucketApiUrl);
  }

  getMonthlyBucketsByYearMonth(year: number, month: number): Observable<MonthlyBucket[]> {
    return this.http.get<MonthlyBucket[]>(`${this.bucketApiUrl}/year/${year}/month/${month}`);
  }

  getMonthlyBucketsByBucketId(bucketId: number): Observable<MonthlyBucket[]> {
    return this.http.get<MonthlyBucket[]>(`${this.bucketApiUrl}/bucket/${bucketId}`);
  }

  generateMonthlyData(request: GenerateMonthlyDataRequest): Observable<void> {
    return this.http.post<void>(`${this.bucketApiUrl}/generate`, request);
  }

  // Monthly Spendings
  getMonthlySpendingById(id: number): Observable<MonthlySpending> {
    return this.http.get<MonthlySpending>(`${this.spendingApiUrl}/${id}`);
  }

  getAllMonthlySpendings(): Observable<MonthlySpending[]> {
    return this.http.get<MonthlySpending[]>(this.spendingApiUrl);
  }

  getMonthlySpendingsByMonthlyBucketId(monthlyBucketId: number): Observable<MonthlySpending[]> {
    return this.http.get<MonthlySpending[]>(`${this.spendingApiUrl}/monthly-bucket/${monthlyBucketId}`);
  }

  getMonthlySpendingsByDateRange(startDate: string, endDate: string): Observable<MonthlySpending[]> {
    return this.http.get<MonthlySpending[]>(`${this.spendingApiUrl}/date-range`, {
      params: { startDate, endDate }
    });
  }

  getMonthlySpendingsByOwner(owner: string): Observable<MonthlySpending[]> {
    return this.http.get<MonthlySpending[]>(`${this.spendingApiUrl}/owner/${owner}`);
  }
}
