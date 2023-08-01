import { Component, OnInit, Output, EventEmitter, Input, Inject } from '@angular/core';
import { HttpEventType, HttpClient, HttpEvent } from '@angular/common/http';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { User } from 'src/app/models/user.model';
import { AccountService } from 'src/app/services/account.service';
import { Utilities } from 'src/app/services/utilities';

@Component({
  selector: 'user-selector',
  templateUrl: './user-selector.component.html',
  styleUrls: ['./user-selector.component.css']
})
export class UserSelectorComponent implements OnInit {
  public userIds: number[];
  users: User[];
  usersCache: User[];
  selectedUsers: User[];
  //@Input() showTxt: boolean;
  //@Output() public onSelectedFinished = new EventEmitter();

  constructor(private accountService: AccountService,
    public dialogRef: MatDialogRef<UserSelectorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.users) != typeof (undefined)) {
      this.selectedUsers = data.users;
    }
  }

  ngOnInit() {
    this.accountService.getUsers().subscribe(results => {
      this.users = results;
      this.users.map(cg => {
        const index = this.selectedUsers.findIndex(item => item.id == cg.id)
        if (index >= 0) {
          cg.checked = true;
        }
      });

      this.usersCache = this.users;
    }, error => { });
  }

  public save = () => {
    this.selectedUsers = this.users.filter((cg) => cg.checked);
    this.dialogRef.close({ isCancel: false, selectedUsers: this.selectedUsers });
  }

  private cancel() {
    this.dialogRef.close({ isCancel: true });
  }

  onSearchChanged(value: string) {
    this.users = this.usersCache.filter(r => Utilities.searchArray(value, false, r.fullName, r.email, r.departmentName, r.jobTitle, r.phoneNumber, r.homeNo, r.mobileNo));
  }
}
