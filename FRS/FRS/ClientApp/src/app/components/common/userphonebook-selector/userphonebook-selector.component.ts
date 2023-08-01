import { Component, OnInit, Output, EventEmitter, Input, Inject } from '@angular/core';
import { HttpEventType, HttpClient, HttpEvent } from '@angular/common/http';
import { UserPhonebook } from 'src/app/models/userPhonebook.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { AccountService } from 'src/app/services/account.service';
import { Utilities } from 'src/app/services/utilities';
import { UserVehicle } from 'src/app/models/uservehicle.model';

@Component({
  selector: 'userphonebook-selector',
  templateUrl: './userphonebook-selector.component.html',
  styleUrls: ['./userphonebook-selector.component.css']
})
export class UserPhonebookSelectorComponent implements OnInit {
  public userPhonebookIds: number[];
  userPhonebooks: UserPhonebook[];
  userPhonebooksCache: UserPhonebook[];
  selectedUserPhonebooks: UserPhonebook[];
  //@Input() showTxt: boolean;
  //@Output() public onSelectedFinished = new EventEmitter();

  constructor(private http: HttpClient, private accountService: AccountService,
    public dialogRef: MatDialogRef<UserPhonebookSelectorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.userPhonebooks) != typeof (undefined)) {
      this.selectedUserPhonebooks = data.userPhonebooks;
    }
  }

  ngOnInit() {
    this.accountService.getUserPhonebooks(this.accountService.currentUser.id).subscribe(results => {
      this.userPhonebooks = results;
      if (typeof (this.selectedUserPhonebooks) != typeof (undefined)) {
        this.userPhonebooks.map(cg => {
          const index = this.selectedUserPhonebooks.findIndex(item => item.email == cg.email)
          if (index >= 0) {
            cg.checked = true;
          }
        });
      }

      this.userPhonebooksCache = this.userPhonebooks;

    }, error => { });
  }

  public save = () => {
    this.selectedUserPhonebooks = this.userPhonebooks.filter((cg) => cg.checked);
    this.dialogRef.close({ isCancel: false, selectedUserPhonebooks: this.selectedUserPhonebooks });
    //this.onSelectedFinished.emit(selectedUserPhonebooks);
  }

  private cancel() {
    this.dialogRef.close({ isCancel: true });
  }

  public setUserVehicle(row: UserPhonebook, vehicle: UserVehicle) {
    row.cardType = vehicle.cardType;
    row.plateNumber = vehicle.plateNumber;
  }

  onSearchChanged(value: string) {
    this.userPhonebooks = this.userPhonebooksCache.filter(r => Utilities.searchArray(value, false, r.name, r.email, r.phoneNumber, r.homeNo, r.mobileNo));
  }
}
