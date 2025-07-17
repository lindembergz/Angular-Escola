import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  private isLocalStorageAvailable(): boolean {
    try {
      const test = '__localStorage_test__';
      localStorage.setItem(test, test);
      localStorage.removeItem(test);
      return true;
    } catch {
      return false;
    }
  }

  setItem(key: string, value: string): void {
    if (this.isLocalStorageAvailable()) {
      localStorage.setItem(key, value);
    }
  }

  getItem(key: string): string | null {
    if (this.isLocalStorageAvailable()) {
      return localStorage.getItem(key);
    }
    return null;
  }

  removeItem(key: string): void {
    if (this.isLocalStorageAvailable()) {
      localStorage.removeItem(key);
    }
  }

  clear(): void {
    if (this.isLocalStorageAvailable()) {
      localStorage.clear();
    }
  }

  // Session Storage methods
  setSessionItem(key: string, value: string): void {
    try {
      sessionStorage.setItem(key, value);
    } catch {
      // Silently fail if sessionStorage is not available
    }
  }

  getSessionItem(key: string): string | null {
    try {
      return sessionStorage.getItem(key);
    } catch {
      return null;
    }
  }

  removeSessionItem(key: string): void {
    try {
      sessionStorage.removeItem(key);
    } catch {
      // Silently fail if sessionStorage is not available
    }
  }

  clearSession(): void {
    try {
      sessionStorage.clear();
    } catch {
      // Silently fail if sessionStorage is not available
    }
  }

  // Utility methods for objects
  setObject(key: string, value: any): void {
    this.setItem(key, JSON.stringify(value));
  }

  getObject<T>(key: string): T | null {
    const item = this.getItem(key);
    if (item) {
      try {
        return JSON.parse(item) as T;
      } catch {
        return null;
      }
    }
    return null;
  }

  setSessionObject(key: string, value: any): void {
    this.setSessionItem(key, JSON.stringify(value));
  }

  getSessionObject<T>(key: string): T | null {
    const item = this.getSessionItem(key);
    if (item) {
      try {
        return JSON.parse(item) as T;
      } catch {
        return null;
      }
    }
    return null;
  }
}