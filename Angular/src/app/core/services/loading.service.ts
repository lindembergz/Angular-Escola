import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  private loadingSubject = new BehaviorSubject<boolean>(false);
  private loadingCountSubject = new BehaviorSubject<number>(0);

  public isLoading$: Observable<boolean> = this.loadingSubject.asObservable();
  public loadingCount$: Observable<number> = this.loadingCountSubject.asObservable();

  show(): void {
    const currentCount = this.loadingCountSubject.value;
    this.loadingCountSubject.next(currentCount + 1);
    this.loadingSubject.next(true);
  }

  hide(): void {
    const currentCount = this.loadingCountSubject.value;
    const newCount = Math.max(0, currentCount - 1);
    this.loadingCountSubject.next(newCount);
    
    if (newCount === 0) {
      this.loadingSubject.next(false);
    }
  }

  forceHide(): void {
    this.loadingCountSubject.next(0);
    this.loadingSubject.next(false);
  }

  isLoading(): boolean {
    return this.loadingSubject.value;
  }
}