import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';


import { fadeInOut } from '../../services/animations';
import { BootstrapTabDirective } from "../../directives/bootstrap-tab.directive";
import { AccountService } from "../../services/account.service";
import { Permission } from '../../models/permission.model';
import { DevicesManagementComponent } from './unknown-device/devices-management.component';


@Component({
  selector: 'device-manager',
  templateUrl: './device-manager.component.html',
  styleUrls: ['./device-manager.css'],
  animations: [fadeInOut]
})
export class DeviceManagerComponent implements OnInit, OnDestroy {


  @ViewChild('deviceManagement')
  deviceManagement: DevicesManagementComponent;


  isUnknownDevice = true;
  isApproveDevice = false;

  fragmentSubscription: any;

  readonly unknownDeviceTab = "unknownDevice";
  readonly approveDeviceTab = "approveDevice";

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
    if ((this.isFragmentEquals(anchor, this.unknownDeviceTab) && !this.canViewDevices) ||
      (this.isFragmentEquals(anchor, this.approveDeviceTab) && !this.canViewDevices))
      return;

    this.tab.show(`#${anchor || this.unknownDeviceTab}Tab`);
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

    this.isUnknownDevice = activeTab == this.unknownDeviceTab;
    this.isApproveDevice = activeTab == this.approveDeviceTab;
    this.deviceManagement.isApproval = this.isApproveDevice;
    
  }

  setIsApproval(isApproval: boolean) {
    this.deviceManagement.isApproval = this.isApproveDevice;
  }

  get canViewDevices() {
    return this.accountService.userHasPermission(Permission.viewDevicesPermission);
  }
}
