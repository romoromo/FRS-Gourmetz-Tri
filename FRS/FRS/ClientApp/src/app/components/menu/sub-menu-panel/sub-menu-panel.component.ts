import { Component, OnInit, ViewChild, Input, EventEmitter, Output, ViewEncapsulation } from "@angular/core";
import { MatMenu } from "@angular/material";
import { Menu } from "src/app/models/menu.model";
import { AccountService } from "src/app/services/account.service";

@Component({
  selector: "app-sub-menu-panel",
  templateUrl: "./sub-menu-panel.component.html",
  styleUrls: ["./sub-menu-panel.component.css"]
})
export class SubMenuPanelComponent implements OnInit {
  @ViewChild("menu") menu: MatMenu;
  @Input() item: Menu;
  @Output('selectMenu')
  submit: EventEmitter<any> = new EventEmitter<any>();
  @Input() selectedMenu: Menu;

  constructor(private accountService: AccountService) { }

  ngOnInit() { }

  selectMenu(menu: Menu) {
    //console.log(this.menu);
    //this.selectedMenu = menu;
    this.submit.emit(menu);
  }

  triggerSelect(menu) {
    //console.log(this.selectedMenu);
    this.submit.emit(menu);
  }

  isAccessAllowed(menu: Menu) {
    return this.accountService.userHasPermissions(menu.permissions);
  }
}
