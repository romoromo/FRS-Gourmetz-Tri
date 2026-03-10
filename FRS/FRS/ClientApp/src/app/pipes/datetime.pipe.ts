import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';


@Pipe({ name: 'dateOnly' })
export class DateOnlyPipe extends DatePipe implements PipeTransform {

    transform(value: any): any {

      return super.transform(value, 'dd/MM/yyyy');
    }
}


@Pipe({ name: 'dateTimeOnly' })
export class DateTimeOnlyPipe extends DatePipe implements PipeTransform {

  transform(value: any): any {

    return super.transform(value, 'dd/MM/yyyy h:mm a');
  }
}

@Pipe({ name: 'timeOnly' })
export class TimeOnlyPipe extends DatePipe implements PipeTransform {

  transform(value: any): any {

    return super.transform(value, 'h:mm a');
  }
}

@Pipe({ name: 'daysBeforeDelivery' })
export class DaysBeforeDeliveryPipe implements PipeTransform {
  transform(deliveryDate: string | Date): string {
    if (!deliveryDate) return '-';

    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const delivery = new Date(deliveryDate);
    delivery.setHours(0, 0, 0, 0);

    const diffMs = delivery.getTime() - today.getTime();
    const diffDays = Math.round(diffMs / (1000 * 60 * 60 * 24));

    if (diffDays > 0) return `${diffDays}d remaining`;
    if (diffDays === 0) return 'Today';
    return `${Math.abs(diffDays)}d overdue`;
  }
}
