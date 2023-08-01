import { Component } from '@angular/core';
import { fadeInOut } from '../../services/animations';
import { ConfigurationService } from '../../services/configuration.service';
import { Permission } from 'src/app/models/permission.model';
import { AccountService } from 'src/app/services/account.service';
import { AuthService } from 'src/app/services/auth.service';


@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css'],
    animations: [fadeInOut]
})
export class HomeComponent {
  constructor(public configurations: ConfigurationService, private accountService: AccountService, private authService: AuthService) {
    
  }

  get canViewDevices() {
    return this.accountService.userHasPermission(Permission.viewDevicesPermission); //Todo: Consider creating separate permission for notifications
  }

  get canViewReservations() {
    return this.accountService.userHasPermission(Permission.addReservationsPermission);
  }

  get canManagePIBDevices() {
    return this.accountService.userHasPermission(Permission.managePIBDevicesTemplatesPermission);
  }

  get canViewDashboard() {
    return this.accountService.userHasPermission(Permission.viewDashboardPermission);
  }

  get canViewSignageDashboard() {
    return this.accountService.userHasPermission(Permission.viewSignageDashboardPermission);
  }
}
