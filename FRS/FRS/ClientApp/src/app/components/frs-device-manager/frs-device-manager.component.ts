import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';


import { fadeInOut } from '../../services/animations';
import { BootstrapTabDirective } from "../../directives/bootstrap-tab.directive";
import { AccountService } from "../../services/account.service";
import { Permission } from '../../models/permission.model';
import { EpaperDevicesManagementComponent } from './frs-unknown-device/frs-devices-management.component';


@Component({
  selector: 'frs-device-manager',
  templateUrl: './frs-device-manager.component.html',
  styleUrls: ['./frs-device-manager.css'],
  animations: [fadeInOut]
})
export class EpaperDeviceManagerComponent implements OnInit, OnDestroy {


  @ViewChild('epaperDeviceManagement')
  epaperDeviceManagement: EpaperDevicesManagementComponent;


  isUnknownEpaperDevice = true;
  isApproveEpaperDevice = false;

  fragmentSubscription: any;

  readonly unknownEpaperDeviceTab = "unknownEpaperDevice";
  readonly approveEpaperDeviceTab = "approveEpaperDevice";

  @ViewChild("tab")
  tab: BootstrapTabDirective;


  constructor(private route: ActivatedRoute, private accountService: AccountService) {
  }


  ngOnInit() {
    this.fragmentSubscription = this.route.fragment.subscribe(anchor => this.showContent(anchor));
  }


  ngOnDestroy() {
    this.fragmentSubscription.unsubscribe();
  }

  showContent(anchor: string) {
    if ((this.isFragmentEquals(anchor, this.unknownEpaperDeviceTab) && !this.canViewEpaperDevices) ||
      (this.isFragmentEquals(anchor, this.approveEpaperDeviceTab) && !this.canViewEpaperDevices))
      return;

    this.tab.show(`#${anchor || this.unknownEpaperDeviceTab}Tab`);
  }


  isFragmentEquals(fragment1: string, fragment2: string) {

    if (fragment1 == null)
      fragment1 = "";

    if (fragment2 == null)
      fragment2 = "";

    return fragment1.toLowerCase() == fragment2.toLowerCase();
  }


  onShowTab(event) {
    let activeTab = event.target.hash.split("#", 2).pop();

    this.isUnknownEpaperDevice = activeTab == this.unknownEpaperDeviceTab;
    this.isApproveEpaperDevice = activeTab == this.approveEpaperDeviceTab;
    this.epaperDeviceManagement.isApproval = this.isApproveEpaperDevice;
    
  }

  setIsApproval(isApproval: boolean) {
    this.epaperDeviceManagement.isApproval = this.isApproveEpaperDevice;
  }

  get canViewEpaperDevices() {
    return this.accountService.userHasPermission(Permission.viewEpaperDevicesPermission);
  }
}
