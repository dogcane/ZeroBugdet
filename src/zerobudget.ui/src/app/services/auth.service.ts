import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { 
  LoginRequest, 
  LoginResponse, 
  RegisterMainUserRequest, 
  User,
  InviteUserRequest,
  UserInvitation,
  CompleteRegistrationRequest
} from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7000/api/account'; // TODO: Configure from environment
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  private tokenKey = 'auth_token';

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.loadUserFromToken();
  }

  private loadUserFromToken(): void {
    const token = this.getToken();
    if (token) {
      // Decode JWT to get user info
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const user: User = {
          id: payload.sub || payload.nameid,
          email: payload.email,
          isMainUser: payload.isMainUser === 'True' || payload.isMainUser === true
        };
        this.currentUserSubject.next(user);
      } catch (e) {
        this.logout();
      }
    }
  }

  isMainUserRequired(): Observable<{ isRequired: boolean }> {
    return this.http.get<{ isRequired: boolean }>(`${this.apiUrl}/is-main-user-required`);
  }

  registerMainUser(request: RegisterMainUserRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register-main-user`, request);
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request).pipe(
      tap(response => {
        this.setToken(response.token);
        const user: User = {
          id: response.userId,
          email: response.email,
          isMainUser: response.isMainUser
        };
        this.currentUserSubject.next(user);
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  isMainUser(): boolean {
    const user = this.getCurrentUser();
    return user?.isMainUser ?? false;
  }

  // User management (main user only)
  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/users`);
  }

  inviteUser(request: InviteUserRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/invite-user`, request);
  }

  getInvitations(): Observable<UserInvitation[]> {
    return this.http.get<UserInvitation[]>(`${this.apiUrl}/invitations`);
  }

  validateInvitationToken(token: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/validate-invitation/${token}`);
  }

  completeRegistration(request: CompleteRegistrationRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/complete-registration`, request);
  }

  deleteUser(userId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/users/${userId}`);
  }
}
