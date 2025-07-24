import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { Router } from '@angular/router';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  expiresAt: string;
}

export interface User {
  id: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  isActive: boolean;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5163/api/auth';
  private currentUserSubject = new BehaviorSubject<LoginResponse | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {
    // Check if user is already logged in on service initialization
    const storedUser = typeof window !== 'undefined' ? window.localStorage.getItem('currentUser') : null;
    if (storedUser) {
      const user = JSON.parse(storedUser);
      // Check if token is still valid
      if (new Date(user.expiresAt) > new Date()) {
        this.currentUserSubject.next(user);
      } else {
        this.logout();
      }
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials)
      .pipe(
        tap(response => {
          if (typeof window !== 'undefined') {
            window.localStorage.setItem('currentUser', JSON.stringify(response));
            window.localStorage.setItem('token', response.token);
          }
          this.currentUserSubject.next(response);
        })
      );
  }

  logout(): void {
    if (typeof window !== 'undefined') {
      window.localStorage.removeItem('currentUser');
      window.localStorage.removeItem('token');
    }
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  isLoggedIn(): boolean {
    const user = this.currentUserSubject.value;
    if (!user) return false;
    
    return new Date(user.expiresAt) > new Date();
  }

  getToken(): string | null {
    return typeof window !== 'undefined' ? window.localStorage.getItem('token') : null;
  }

  getCurrentUser(): LoginResponse | null {
    return this.currentUserSubject.value;
  }

  getProfile(): Observable<User> {
    const headers = new HttpHeaders().set('Authorization', `Bearer ${this.getToken()}`);
    return this.http.get<User>(`${this.apiUrl}/profile`, { headers });
  }

  validateToken(): Observable<any> {
    const headers = new HttpHeaders().set('Authorization', `Bearer ${this.getToken()}`);
    return this.http.post(`${this.apiUrl}/validate`, {}, { headers });
  }
}
