import { Component } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { RentalRequest, ToastNotification, ChefModel } from './chef.model';
import { ChefService } from './chef.service';

@Component({
  selector: 'app-chef',
  imports: [],
  templateUrl: './chef.html',
  styleUrl: './chef.css',
})
export class Chef {
  chefs: ChefModel[] = [];
  filteredChefs: ChefModel[] = [];
  chefSearchQuery = '';
  selectedChef: ChefModel | null = null;
  showModal = false;
  
  rentalForm = {
    startDate: '',
    endDate: '',
    duration: 0,
    totalPrice: 0
  };

  rentalLoading = false;
  notifications: ToastNotification[] = [];
  
  private destroy$ = new Subject<void>();

  constructor(
      private chefService: ChefService
    ) {}

     ngOnInit(): void {
        this.chefService.getChefs()
          .pipe(takeUntil(this.destroy$))
          .subscribe(chefs => {
            this.chefs = chefs;
            this.filteredChefs = chefs;
          });
    
        this.chefService.notifications$
          .pipe(takeUntil(this.destroy$))
          .subscribe(notification => {
            this.notifications.push(notification);
            setTimeout(() => {
              this.notifications = this.notifications.filter(n => n.id !== notification.id);
            }, notification.duration || 3000);
          });
      }

       // Chef list methods
  onChefSearch(query: string): void {
    this.chefSearchQuery = query;
    this.chefService.filterChefs(query)
      .pipe(takeUntil(this.destroy$))
      .subscribe(chefs => {
        this.filteredChefs = chefs;
      });
  }

  openRentalModal(chef: ChefModel): void {
    this.selectedChef = chef;
    this.showModal = true;
    this.rentalForm = {
      startDate: '',
      endDate: '',
      duration: 0,
      totalPrice: 0
    };
  }

  closeModal(): void {
    this.showModal = false;
    this.selectedChef = null;
  }

  calculatePrice(): void {
    if (this.rentalForm.startDate && this.rentalForm.endDate) {
      const start = new Date(this.rentalForm.startDate);
      const end = new Date(this.rentalForm.endDate);
      const duration = Math.floor((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24)) + 1;

      if (duration < 3) {
        this.chefService.warning('A bérlés időtartama legalább 3 napnak kell lennie!');
        this.rentalForm.duration = 0;
        return;
      }
      if (duration > 14) {
        this.chefService.warning('A bérlés időtartama legfeljebb 14 nap lehet!');
        this.rentalForm.duration = 0;
        return;
      }

      this.rentalForm.duration = duration;
      if (this.selectedChef) {
        this.rentalForm.totalPrice = 
          this.selectedChef.baseFee + (this.selectedChef.dailyRate * duration);
      }
    }
  }

  submitRental(): void {
    if (!this.selectedChef || !this.rentalForm.startDate || !this.rentalForm.endDate) {
      this.chefService.error('Kérjük, töltsön ki minden mezőt!');
      return;
    }

    this.rentalLoading = true;
    const rentalRequest: RentalRequest = {
      uid: 101,
      chefId: this.selectedChef.id,
      startDate: this.rentalForm.startDate,
      endDate: this.rentalForm.endDate,
      dailyRate: this.selectedChef.dailyRate,
      baseFee: this.selectedChef.baseFee
    };

    this.chefService.createRental(rentalRequest)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.rentalLoading = false;
          this.chefService.success(
            `Bérlés sikeresen rögzítve! Bérlés ID: #${response.id}`
          );
          this.closeModal();
        },
        error: (error) => {
          this.rentalLoading = false;
          const errorMessage = error.error?.message || error.message || 'Hiba történt a bérlés során';
          this.chefService.error(errorMessage);
        }
      });
  }

  getMinDate(): string {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    return tomorrow.toISOString().split('T')[0];
  }
}

