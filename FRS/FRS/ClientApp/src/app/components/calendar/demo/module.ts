import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { DemoUtilsModule } from './demo-utils/module';
import { DemoComponent } from './component';
import { DayViewSchedulerComponent } from './day-view-scheduler.component';

@NgModule({
  imports: [
    CommonModule,
    CalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory
    }),
    DemoUtilsModule
  ],
  declarations: [DemoComponent, DayViewSchedulerComponent],
  exports: [DemoComponent]
})
export class DemoModule {}
