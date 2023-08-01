import { Component, OnInit, Output, EventEmitter, Input, Inject } from '@angular/core';
import { HttpEventType, HttpClient, HttpEvent } from '@angular/common/http';
import { ContactGroupService } from 'src/app/services/contactgroup.service';
import { ContactGroup } from 'src/app/models/contactGroup.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Utilities } from 'src/app/services/utilities';
import { AccountService } from 'src/app/services/account.service';
import { AlertService } from 'src/app/services/alert.service';

@Component({
  selector: 'contact-group-selector',
  templateUrl: './contact-group-selector.component.html',
  styleUrls: ['./contact-group-selector.component.css']
})
export class ContactGroupSelectorComponent implements OnInit {
  public contactGroupIds: number[];
  contactGroups: ContactGroup[];
  contactGroupsCache: ContactGroup[];
  selectedContactGroups: ContactGroup[];
  //@Input() showTxt: boolean;
  //@Output() public onSelectedFinished = new EventEmitter();

  constructor(private http: HttpClient, private alertService: AlertService, private contactGroupService: ContactGroupService, private accountService: AccountService,
    public dialogRef: MatDialogRef<ContactGroupSelectorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.contactGroups) != typeof (undefined)) {
      this.selectedContactGroups = data.contactGroups;
    }

    this.alertService.startLoadingMessage("Loading contact groups...");
    this.contactGroupService.getContactGroups(null, null, this.accountService.currentUser.institutionId, true).subscribe(results => {
      this.alertService.stopLoadingMessage();
      this.contactGroups = results;
      this.contactGroups.map(cg => {
        if (this.selectedContactGroups) {
          const index = this.selectedContactGroups.findIndex(item => item.id == cg.id)
          if (index >= 0) {
            cg.checked = true;
          }
        }
      });
      this.contactGroupsCache = this.contactGroups;
    }, error => { });
  }

  ngOnInit() {
    
  }

  public save = () => {
    this.selectedContactGroups = this.contactGroups.filter((cg) => cg.checked);
    this.dialogRef.close({ isCancel: false, selectedContactGroups: this.selectedContactGroups });
    //this.onSelectedFinished.emit(selectedContactGroups);
  }

  private cancel() {
    this.dialogRef.close({ isCancel: true });
  }

  onSearchChanged(value: string) {
    if (this.contactGroupsCache) {
      this.contactGroups = this.contactGroupsCache.filter(r => Utilities.searchArray(value, false, r.name, r.description));
    }
  }
}
