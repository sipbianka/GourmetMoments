export interface ChefModel {
  id: number;
  name: string;
  cuisine: string;
  experience: number;
  dailyRate: number;
  baseFee: number;
  image?: string;
}

export interface ToastNotification {
  id: string;
  type: 'success' | 'error' | 'warning';
  message: string;
  duration?: number;
}

export interface RentalRequest {
  uid: number;
  chefId: number;
  startDate: string;
  endDate: string;
  dailyRate: number;
  baseFee: number;
}

export interface RentalResponse {
  id: number;
  uid: number;
  chefId: number;
  startDate: string;
  endDate: string;
  dailyRate: number;
  baseFee: number;
  totalPrice: number;
}

