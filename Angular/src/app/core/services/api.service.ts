import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
  errors?: string[];
}

export interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface QueryParams {
  [key: string]: string | number | boolean | undefined;
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly baseUrl = environment.apiUrl || 'http://localhost:5000/api';

  constructor(private http: HttpClient) {}

  // GET requests
  get<T>(endpoint: string, params?: QueryParams): Observable<ApiResponse<T>> {
    const httpParams = this.buildHttpParams(params);
    return this.http.get<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`, { params: httpParams });
  }

  getPaginated<T>(endpoint: string, params?: QueryParams): Observable<ApiResponse<PaginatedResponse<T>>> {
    const httpParams = this.buildHttpParams(params);
    return this.http.get<ApiResponse<PaginatedResponse<T>>>(`${this.baseUrl}/${endpoint}`, { params: httpParams });
  }

  getById<T>(endpoint: string, id: string | number): Observable<ApiResponse<T>> {
    return this.http.get<ApiResponse<T>>(`${this.baseUrl}/${endpoint}/${id}`);
  }

  // POST requests
  post<T>(endpoint: string, data: any): Observable<ApiResponse<T>> {
    return this.http.post<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`, data);
  }

  // PUT requests
  put<T>(endpoint: string, id: string | number, data: any): Observable<ApiResponse<T>> {
    return this.http.put<ApiResponse<T>>(`${this.baseUrl}/${endpoint}/${id}`, data);
  }

  // PATCH requests
  patch<T>(endpoint: string, id: string | number, data: any): Observable<ApiResponse<T>> {
    return this.http.patch<ApiResponse<T>>(`${this.baseUrl}/${endpoint}/${id}`, data);
  }

  // DELETE requests
  delete<T>(endpoint: string, id: string | number): Observable<ApiResponse<T>> {
    return this.http.delete<ApiResponse<T>>(`${this.baseUrl}/${endpoint}/${id}`);
  }

  // File upload
  uploadFile<T>(endpoint: string, file: File, additionalData?: any): Observable<ApiResponse<T>> {
    const formData = new FormData();
    formData.append('file', file);
    
    if (additionalData) {
      Object.keys(additionalData).forEach(key => {
        formData.append(key, additionalData[key]);
      });
    }

    return this.http.post<ApiResponse<T>>(`${this.baseUrl}/${endpoint}`, formData);
  }

  // Download file
  downloadFile(endpoint: string, filename?: string): Observable<Blob> {
    const headers = new HttpHeaders({
      'Accept': 'application/octet-stream'
    });

    return this.http.get(`${this.baseUrl}/${endpoint}`, {
      headers,
      responseType: 'blob'
    });
  }

  private buildHttpParams(params?: QueryParams): HttpParams {
    let httpParams = new HttpParams();
    
    if (params) {
      Object.keys(params).forEach(key => {
        const value = params[key];
        if (value !== undefined && value !== null) {
          httpParams = httpParams.set(key, value.toString());
        }
      });
    }
    
    return httpParams;
  }
}