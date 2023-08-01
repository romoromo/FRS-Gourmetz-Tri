import { Component, OnInit, OnDestroy, Input, ViewEncapsulation } from "@angular/core";

import { AlertService, MessageSeverity, DialogType } from '../../services/alert.service';
import { AuthService } from "../../services/auth.service";
import { ConfigurationService } from '../../services/configuration.service';

import { ActivatedRoute } from "@angular/router";
import { LocalStoreManager } from "src/app/services/local-store-manager.service";

@Component({
  selector: "device-display-warning",
  templateUrl: './device-warning.component.html',
  styleUrls: ['./device-warning.component.css'],
  encapsulation: ViewEncapsulation.None
})

export class DeviceDisplayWarningComponent implements OnInit, OnDestroy {
  msg: string;
  errorMessage: string;
  device_id: string;
  mac_address: string;

  constructor(private alertService: AlertService, private authService: AuthService, private configurations: ConfigurationService,
    private route: ActivatedRoute, private storeManager: LocalStoreManager) {
    this.route.params.subscribe(queryParams => {
      this.msg = queryParams["msg"];
      this.device_id = queryParams["id"];
      this.mac_address = queryParams["address"];
      this.setErrorMessage();
    });

    if (!this.msg) {
      this.route.queryParams.subscribe(queryParams => {
        this.errorMessage = queryParams["mac_address"];
        this.device_id = queryParams["id"];
        this.mac_address = queryParams["address"];
        this.setErrorMessage();
      });
    }

    //this.route.params.subscribe(queryParams => {
    //  this.msg = queryParams.get("msg");
    //  this.device_id = queryParams.get("id");
    //  this.errorMessage = "";
    //  if (this.msg == "adminreq") {
    //    this.errorMessage = "Device needs to be setup. Please contact administrator.";
    //  } else if (this.msg == "new_error") {
    //    this.errorMessage = "An error occurred during device registration. Please contact administrator.";
    //  } else if (this.msg == "macreq") {
    //    this.errorMessage = "Please provide the device MAC Address.";
    //  }
    //});
  }

  setErrorMessage() {
    this.errorMessage = "";
    if (this.msg == "adminreq") {
      this.errorMessage = "Device " + (this.mac_address ? "(" + this.mac_address + ") " : "") + "needs to be setup. Please contact administrator.\n";
    } else if (this.msg == "new_error") {
      this.errorMessage = "An error occurred during device registration. Please contact administrator.";
    } else if (this.msg == "macreq") {
      this.errorMessage = "Please provide the device MAC Address.";
    }
  }

  ngOnInit() {

  }


  ngOnDestroy() {

  }
}
