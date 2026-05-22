import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { ChefModel, RentalRequest, RentalResponse, ToastNotification } from './chef.model';

@Injectable({
  providedIn: 'root'
})
export class ChefService {
  private chefs$ = new BehaviorSubject<ChefModel[]>([]);
  private readonly FIREBASE_URL = 'https://p161-7ddfd-default-rtdb.europe-west1.firebasedatabase.app/chefs.json';
  private readonly API_URL = 'http://localhost:5205/api/berlesek';
    private notificationsSubject = new Subject<ToastNotification>();
  public notifications$ = this.notificationsSubject.asObservable();
  
  constructor(private http: HttpClient) {
    this.loadChefs();
  }

  private loadChefs(): void {
    this.http.get<{ [key: string]: ChefModel }>(this.FIREBASE_URL).subscribe({
      next: (data) => {
        const chefs = Object.values(data || {});
        this.chefs$.next(chefs);
      },
      error: (err) => console.error('Error loading chefs:', err)
    });
  }

  getChefs(): Observable<ChefModel[]> {
    return this.chefs$.asObservable();
  }

  filterChefs(query: string): Observable<ChefModel[]> {
    return new Observable(observer => {
      this.chefs$.subscribe(chefs => {
        const filtered = chefs.filter(chef =>
          chef.name.toLowerCase().includes(query.toLowerCase()) ||
          chef.cuisine.toLowerCase().includes(query.toLowerCase())
        );
        observer.next(filtered);
      });
    });
  }

    createRental(rental: RentalRequest): Observable<RentalResponse> {
    return this.http.post<RentalResponse>(this.API_URL, {
      ...rental,
      startDate: new Date(rental.startDate).toISOString(),
      endDate: new Date(rental.endDate).toISOString()
    });
  }


  show(type: 'success' | 'error' | 'warning', message: string, duration = 3000): void {
    const id = Date.now().toString();
    const notification: ToastNotification = { id, type, message, duration };
    this.notificationsSubject.next(notification);

    if (duration > 0) {
      setTimeout(() => this.hide(id), duration);
    }
  }

  hide(id: string): void {

  }

  success(message: string, duration?: number): void {
    this.show('success', message, duration);
  }

  error(message: string, duration?: number): void {
    this.show('error', message, duration);
  }

  warning(message: string, duration?: number): void {
    this.show('warning', message, duration);
  }
}