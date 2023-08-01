import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';


import { fadeInOut } from '../../services/animations';
import { BootstrapTabDirective } from "../../directives/bootstrap-tab.directive";
import { AccountService } from "../../services/account.service";
import { Permission } from '../../models/permission.model';
import { PIBDevicesManagementComponent } from './pib-unknown-device/pib-devices-management.component';


@Component({
  selector: 'pib-device-manager',
  templateUrl: './pib-device-manager.component.html',
  styleUrls: ['./pib-device-manager.css'],
  animations: [fadeInOut]
})
export class PIBDeviceManagerComponent implements OnInit, OnDestroy {


  @ViewChild('pibDeviceManagement')
  pibDeviceManagement: PIBDevicesManagementComponent;


  isUnknownPIBDevice = true;
  isApprovePIBDevice = false;

  fragmentSubscription: any;

  readonly unknownPIBDeviceTab = "unknownPIBDevice";
  readonly approvePIBDeviceTab = "approvePIBDevice";

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
    if ((this.isFragmentEquals(anchor, this.unknownPIBDeviceTab) && !this.canViewPIBDevices) ||
      (this.isFragmentEquals(anchor, this.approvePIBDeviceTab) && !this.canViewPIBDevices))
      return;

    this.tab.show(`#${anchor || this.unknownPIBDeviceTab}Tab`);
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

    this.isUnknownPIBDevice = activeTab == this.unknownPIBDeviceTab;
    this.isApprovePIBDevice = activeTab == this.approvePIBDeviceTab;
    this.pibDeviceManagement.isApproval = this.isApprovePIBDevice;
    
  }

  setIsApproval(isApproval: boolean) {
    this.pibDeviceManagement.isApproval = this.isApprovePIBDevice;
  }

  get canViewPIBDevices() {
    return true;//this.accountService.userHasPermission(Permission.viewPIBDevicesPermission);
  }
}
