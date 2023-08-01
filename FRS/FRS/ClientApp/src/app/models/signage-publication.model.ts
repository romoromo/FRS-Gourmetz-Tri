import { Permission } from './permission.model';
import { SignageComponent } from './signage-component.model';
import { SignageSchedule } from './signage-schedule.model';


export class SignagePublication {

  public id: string;
  public name: string;
  public description: string;
  public approved: boolean;

  schedules: SignageSchedule[];
    rejected: boolean;
    remark: any;
    approvedDate?: any;
    rejectedDate: any;
    approvedByUser: any;
    approvedByName: any;
    rejectedByUser: any;
    rejectedByName: any;
}
