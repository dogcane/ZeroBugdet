import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Bucket, CreateBucketCommand, UpdateBucketRequest } from '../models/bucket.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class BucketService {
  private apiUrl = `${environment.apiUrl}/bucket`;

  constructor(private http: HttpClient) {}

  getById(id: number): Observable<Bucket> {
    return this.http.get<Bucket>(`${this.apiUrl}/${id}`);
  }

  getByName(name: string = '', description: string = '', enabled: boolean = true): Observable<Bucket[]> {
    const params: any = { enabled };
    if (name) params.name = name;
    if (description) params.description = description;
    return this.http.get<Bucket[]>(this.apiUrl, { params });
  }

  create(command: CreateBucketCommand): Observable<Bucket> {
    return this.http.post<Bucket>(this.apiUrl, command);
  }

  update(id: number, request: UpdateBucketRequest): Observable<Bucket> {
    return this.http.put<Bucket>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  enable(id: number): Observable<Bucket> {
    return this.http.patch<Bucket>(`${this.apiUrl}/${id}/enable`, {});
  }
}
