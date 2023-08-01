import { Permission } from './permission.model';
import { SignageComponent } from './signage-component.model';
import { SignageCompilation } from './signage-compilation.model';


export class SignageSchedule {

  public id: string;
  public name: string;
  public description: string;

  public publicationId: string;

  public effectiveDate: string;
  public ineffectiveDate: string;

  public startTime: string;
  public endTime: string;

  public startTimeObj: string;
  public endTimeObj: string;

  public monday: boolean;
  public tuesday: boolean;
  public wednesday: boolean;
  public thursday: boolean;
  public friday: boolean;
  public saturday: boolean;
  public sunday: boolean;

  compilations: SignageCompilation[];

  isActive: any;
    effectiveDateObj: Date;
    ineffectiveDateObj: Date;
}
