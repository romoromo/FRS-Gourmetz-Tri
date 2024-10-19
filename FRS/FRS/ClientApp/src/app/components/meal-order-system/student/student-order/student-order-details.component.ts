import { ViewChild, Component, Inject, OnInit, OnDestroy } from "@angular/core";
import { AlertService } from "src/app/services/alert.service";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material";
import { Subscription } from "rxjs";
import { TokenOrder } from "src/app/models/meal-order/token-order.model";
import { DefaultPipe } from "../../../../pipes/default-string-val.pipe";

@Component({
  selector: 'student-order-details',
  templateUrl: './student-order-details.component.html',
  styleUrls: ['./student-order-details.component.css']
})
export class StudentOrderDetailComponent implements OnInit, OnDestroy{
  private subscription: Subscription = new Subscription();
  private order: TokenOrder;
  
  constructor(private alertService: AlertService, 
    public dialogRef: MatDialogRef<StudentOrderDetailComponent>, public dialog: MatDialog, 
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.order = data.order;

    if (this.order.tokens && this.order.tokens.length > 0) {
      let meal = this.order.tokens[0];
      if (meal.selectedDishes && meal.selectedDishes.length > 0) {
        this.order.mealDescription = meal.selectedDishes[0].dishLabel;
      }
    }
  }

  ngOnInit() {
    this.alertService.resetStickyMessage();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  private cancel() {

    this.alertService.resetStickyMessage();

    this.dialogRef.close();
  }

  get canManageStudents() {
    return true; //this.accountService.userHasPermission(Permission.manageStudentsPermission)
  }
}
