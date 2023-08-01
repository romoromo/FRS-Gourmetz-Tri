import { Permission } from './permission.model';
import { SignageComponent } from './signage-component.model';
import { SignageCompilation } from './signage-compilation.model';


export class EmployeeScheduleShift {

  public id: string;
  public name: string;

  public scheduleId: string;

  public startTime: string;
  public endTime: string;

  public monday: boolean;
  public tuesday: boolean;
  public wednesday: boolean;
  public thursday: boolean;
  public friday: boolean;
  public saturday: boolean;
  public sunday: boolean;

  isActive: any;
}
