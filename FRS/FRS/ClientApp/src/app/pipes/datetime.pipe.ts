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
