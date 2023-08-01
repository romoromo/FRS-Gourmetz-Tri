import { Component, OnInit, ViewChild, Input, EventEmitter, Output } from "@angular/core";
import { MatMenu, MatSelect } from "@angular/material";

@Component({
  selector: "app-location-panel",
  templateUrl: "./location-panel.component.html",
  styleUrls: ["./location-panel.component.css"]
})
export class LocationPanelComponent implements OnInit {
  @ViewChild("select") menu: MatSelect;
  @Input() items: any[];
  @Output('submit')
  submit: EventEmitter<any> = new EventEmitter<any>();
  @Input() selectedMenu: any;

  constructor() { }

  ngOnInit() { }

  submitActivity(menu: any) {
    //console.log(this.menu);
    //this.selectedMenu = menu;
    this.submit.emit(menu);
  }

  triggerSubmit(menu) {
    //console.log(this.selectedMenu);
    this.submit.emit(menu);
  }
}
