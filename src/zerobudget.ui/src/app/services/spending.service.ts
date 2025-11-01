import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Spending, CreateSpendingCommand, UpdateSpendingRequest } from '../models/spending.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SpendingService {
  private apiUrl = `${environment.apiUrl}/spending`;

  constructor(private http: HttpClient) { }

  getById(id: number): Observable<Spending> {
    return this.http.get<Spending>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get spendings with optional filters for bucket, description, owner, and enabled status
   * Uses GetSpendingsQuery handler with optional parameters
   */
  getAll(bucketId?: number, description?: string, owner?: string, enabled?: boolean): Observable<Spending[]> {
    let params = new HttpParams();

    if (bucketId !== undefined) {
      params = params.set('bucketId', bucketId.toString());
    }
    if (description !== undefined && description.length > 0) {
      params = params.set('description', description);
    }
    if (owner !== undefined && owner.length > 0) {
      params = params.set('owner', owner);
    }
    if (enabled !== undefined) {
      params = params.set('enabled', enabled.toString());
    }

    return this.http.get<Spending[]>(this.apiUrl, { params });
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

