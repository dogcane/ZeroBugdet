import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Spending, CreateSpendingCommand, UpdateSpendingRequest } from '../models/spending.model';

@Injectable({
  providedIn: 'root'
})
export class SpendingService {
  private apiUrl = 'https://localhost:7000/api/spending'; // TODO: Configure from environment

  constructor(private http: HttpClient) {}

  getById(id: number): Observable<Spending> {
    return this.http.get<Spending>(`${this.apiUrl}/${id}`);
  }

  getAll(): Observable<Spending[]> {
    return this.http.get<Spending[]>(this.apiUrl);
  }

  getByBucketId(bucketId: number): Observable<Spending[]> {
    return this.http.get<Spending[]>(`${this.apiUrl}/bucket/${bucketId}`);
  }

  getByOwner(owner: string): Observable<Spending[]> {
    return this.http.get<Spending[]>(`${this.apiUrl}/owner/${owner}`);
  }

  create(command: CreateSpendingCommand): Observable<Spending> {
    return this.http.post<Spending>(this.apiUrl, command);
  }

  update(id: number, request: UpdateSpendingRequest): Observable<Spending> {
    return this.http.put<Spending>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  enable(id: number): Observable<Spending> {
    return this.http.patch<Spending>(`${this.apiUrl}/${id}/enable`, {});
  }
}
