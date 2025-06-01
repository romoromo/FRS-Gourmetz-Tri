import { Component, Inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { forkJoin, Subscription } from 'rxjs';
import { Class } from 'src/app/models/meal-order/class.model';
import { Permission } from 'src/app/models/permission.model';
import { Filter } from 'src/app/models/sieve-filter.model';
import { AccountService } from 'src/app/services/account.service';
import { AlertService, MessageSeverity } from 'src/app/services/alert.service';
import { ClassService } from 'src/app/services/meal-order/class.service';
import { StudentService } from 'src/app/services/meal-order/student.service';

@Component({
  selector: 'app-class-transfer',
  templateUrl: './class-transfer.component.html',
  styleUrls: ['./class-transfer.component.css'],
})
export class ClassTransferComponent implements OnInit, OnDestroy {
  private subscription: Subscription = new Subscription();
  originClassName: string = ""
  originClassId: number = 0
  showValidationErrors: boolean = true;
  private outletId: string;
  batches = [];
  classLevels = [];
  classes = [];
  loadingInitialFormData: boolean = false;
  isSaving: boolean = false;

  @ViewChild('f')
  private formTransfer: NgForm;

  constructor(
    private alertService: AlertService,
    private classService: ClassService,
    private studentService: StudentService,
    public dialogRef: MatDialogRef<ClassTransferComponent>,
    private accountService: AccountService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    if (data && data.classModel) {
      this.originClassName = data.classModel.name;
      this.originClassId = data.classModel.id;
      this.outletId = data.outletId;
      this.loadDropdownData()
    }
  }

  ngOnInit() {
    this.alertService.resetStickyMessage();
  }

  ngOnDestroy() {
    this.alertService.resetStickyMessage();
    this.subscription.unsubscribe();
  }

  loadDropdownData() {
    this.loadingInitialFormData = true;

    forkJoin([
      this.getClassBatches(),
      this.getClassLevels()
    ]).subscribe({
      next: ([batches, levels]) => {
        this.batches = batches.pagedData;
        this.classLevels = levels.pagedData;
      },
      error: (err) => {
        this.alertService.showStickyMessage("Get Error", `An error occured while retrieving Batch or Class Level.\r\n"`, MessageSeverity.error);
      },
      complete: () => {
        this.loadingInitialFormData = false;
      }
    });
  }

  getClassBatches() {
    let filter = new Filter();
    let f = this.outletId ? '(OutletId)==' + this.outletId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    return this.classService.getClassBatchesByFilter(filter);
  }

  getClassLevels() {
    this.loadingInitialFormData = true;
    let filter = new Filter();
    let f = this.outletId ? '(OutletId)==' + this.outletId + ',' : '';
    filter.filters = f + '(IsActive)==true';
    return this.classService.getClassLevelsByFilter(filter);
  }

  getClasses(classLevelId?: string) {
    if (classLevelId) {
      this.loadingInitialFormData = true;
      let filter = new Filter();
      filter.filters = '(IsActive)==true,(ClassLevelId)==' + classLevelId;

      this.subscription.add(this.classService.getClassesByFilter(filter)
        .subscribe(results => {
          const filteredData = results.pagedData.filter(x => x.id != this.originClassId)
          this.classes = filteredData;
        },
          error => {
            this.alertService.showStickyMessage("Get Error", `An error occured while retrieving classes.\r\n"`,
              MessageSeverity.error);
          }, () => {
            this.loadingInitialFormData = false;
          }));
    } else {
      this.classes = [];
    }
  }

  onClassLevelChange($event: any) {
    const classLevelId = $event.value;
    this.formTransfer.controls['classId'].setValue(null);
    this.getClasses(classLevelId);

  }

  onSave() {
    if (this.formTransfer.valid) {
      this.isSaving = true;
      const formData = this.formTransfer.value;
      this.alertService.startLoadingMessage("Transferring class...");
      const dataRequest = {
        classId: formData.classId,
        classLevelId: formData.classLevelId,
        classBatchId: formData.batchId,
      }
      this.subscription.add(this.studentService.transferClassStudent(dataRequest, this.originClassId)
        .subscribe(
          (response: any) => {
            if (response.isSuccess) {
              this.alertService.showMessage("Success", `Class "${this.originClassName}" has been successfully transferred.`, MessageSeverity.success);
              this.dialogRef.close(true);
            } else {
              this.alertService.showMessage("Failed", `${response.message}`, MessageSeverity.error);
            }
          },
          error => {
            this.alertService.showStickyMessage("Transfer Error", `An error occurred while transferring the class.\r\n${error}`,
              MessageSeverity.error);
            this.isSaving = false;
          },
          () => {
            this.isSaving = false;
            this.formTransfer.reset();
            this.alertService.stopLoadingMessage();
          }
        ));
    }
  }

  private cancel() {
    this.alertService.resetStickyMessage();
    this.dialogRef.close();
    this.isSaving = false;
    this.formTransfer.reset();
  }

  get canManageClasses() {
    return this.accountService.userHasPermission(Permission.manageMOSOutletMgtClassesPermission)
  }
}
