import { Component, ViewChild, Inject, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { SignageCompilationService } from "../../../services/signage-compilation.service";
import { SignageCompilation } from '../../../models/signage-compilation.model';
import { Permission } from '../../../models/permission.model';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { SignageComponentService } from '../../../services/signage-component.service';
import { SignageComponent } from '../../../models/signage-component.model';
import { CdkDragEnd } from '@angular/cdk/drag-drop';
import { FormControl } from '@angular/forms';


@Component({
  selector: 'signage-compilation-editor',
  templateUrl: './signage-compilation-editor.component.html',
  styleUrls: ['./signage-compilation-editor.component.css']
})
export class SignageCompilationEditorComponent {

  private isNewSignageCompilation = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingSignageCompilationName: string;
  private signageCompilationEdit: SignageCompilation = new SignageCompilation();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};

  public formResetToggle = true;

  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;

  public searchForm: FormControl = new FormControl();

  @ViewChild('f')
  private form;

  @ViewChild('compilationContainer') elementView;

  scale: number;

  signageComponents: SignageComponent[];

  constructor(private alertService: AlertService, private signageCompilationService: SignageCompilationService, private accountService: AccountService, private cdRef: ChangeDetectorRef, signageComponentService: SignageComponentService,
    //public dialogRef: MatDialogRef<SignageCompilationEditorComponent>,
  ) {

    signageComponentService.getSignageComponents()
      .subscribe(results => {
        this.signageComponents = results[0];
      },
        error => {
          this.alertService.showStickyMessage("Load Error", `Unable to retrieve signage components from the server.\r\nErrors:`,
            MessageSeverity.error);
        });
  }

  ngAfterViewInit() {

    this.cdRef.detectChanges();
  }

  dragEnded(event, c, w, h) {
    console.log(event);

    //    const transform = event.source.element.nativeElement.style.transform;

    //  let regex = /translate3d\(\s?(?<x>[-]?\d*)px,\s?(?<y>[-]?\d*)px,\s?(?<z>[-]?\d*)px\)/;
    //var values = regex.exec(transform);
    //console.log(transform);
    const offset = { x: event.offsetX, y: event.offsetY };

    const position: any = {};

    position.x = c.x + (offset.x / this.scale);
    position.y = c.y + (offset.y / this.scale);

    //console.log(position, { x: c.x, y: c.y }, offset);

    console.log("PAGE", event.pageX, event.pageY)
    console.log("CLIENT", event.clientX, event.clientY)

    //c.x = position.x;
    //c.y = position.y;
    //c.width = event.srcElement.offsetWidth / this.scale;
    //c.height = event.srcElement.offsetHeight / this.scale;

    //if (c.x + c.width > w) c.x = w - c.width;
    //if (c.y + c.height > h) c.y = h - c.height;

    //if (c.x < 0) c.x = 0;
    //if (c.y < 0) c.y = 0;

    //event.source.element.nativeElement.style.transform = "translate3d(0, 0, 0)";
  }

  add(selected: SignageComponent) {
    if (!selected) return;
    if (!this.signageCompilationEdit.components) this.signageCompilationEdit.components = [];

    let c = new SignageComponent();
    Object.assign(c, selected);
    c.x = 0;
    c.y = 0;
    c.width = 100;
    c.height = 100;
    c.order = 1;
    c.isActive = 1;

    if (this.signageCompilationEdit.components[0]) {
      let lastc = this.signageCompilationEdit.components[this.signageCompilationEdit.components.length - 1];
      c.order = lastc.order + 1;
    }

    c.componentId = c.id;
    c.id = "0";
    this.signageCompilationEdit.components.push(c);
  }

  getStr(obj: any) {
    return JSON.stringify(obj);
  }

  xChanged(event, c, w) {
    //if (c.x > w) c.x = w - c.width;
    //if (c.x + c.width > w) c.width = w - c.x;
  }

  yChanged(event, c, h) {
    //if (c.y > h) c.y = h - c.height;
    //if (c.y + c.height > h) c.height = h - c.y;
  }

  wChanged(event, c, w) {
    //if (c.width > w) c.width = w - c.x;
    //if (c.x + c.width > w) c.x = w - c.width;
  }

  hChanged(event, c, h) {
    //if (c.height > h) c.height = h - c.y;
    //if (c.y + c.height > h) c.y = h - c.height;
  }

  getHeight() {
    try {
      this.scale = this.elementView.nativeElement.offsetWidth / this.signageCompilationEdit.width;

      let h = this.elementView.nativeElement.offsetWidth / this.signageCompilationEdit.width * this.signageCompilationEdit.height;
      if (h) return h;
    } catch (ex) { }

    return 100;
  }

  public uploadFinished = (event) => {
    this.signageCompilationEdit.backgroundImage = event ? event.dbPath : null;
  }


  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");

    for (let c of this.signageCompilationEdit.components || []) {
      c.x = c.x || 0;
      c.y = c.y || 0;
      c.width = c.width || 0;
      c.height = c.height || 0;
      c.order = c.order || 0;
    }


    if (this.isNewSignageCompilation) {
      this.signageCompilationService.newSignageCompilation(this.signageCompilationEdit).subscribe(signageCompilation => this.saveSuccessHelper(signageCompilation), error => this.saveFailedHelper(error));
    }
    else {
      this.signageCompilationService.updateSignageCompilation(this.signageCompilationEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }




  private saveSuccessHelper(signageCompilation?: SignageCompilation) {
    if (signageCompilation)
      Object.assign(this.signageCompilationEdit, signageCompilation);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewSignageCompilation)
      this.alertService.showMessage("Success", `Signage Compilation \"${this.signageCompilationEdit.name}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to signage compilation \"${this.signageCompilationEdit.name}\" was saved successfully`, MessageSeverity.success);


    this.signageCompilationEdit = new SignageCompilation();
    this.resetForm();


    if (this.changesSavedCallback)
      this.changesSavedCallback();

    //this.dialogRef.close();
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();

    //this.dialogRef.close();
  }


  private cancel() {
    this.signageCompilationEdit = new SignageCompilation();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    //this.dialogRef.close();
  }

  private toggleGroup(groupName: string) {
    let firstMemberValue: boolean;

    this.allPermissions.forEach(p => {
      if (p.groupName != groupName)
        return;

      if (firstMemberValue == null)
        firstMemberValue = this.selectedValues[p.value] == true;

      this.selectedValues[p.value] = !firstMemberValue;
    });
  }


  private getSelectedPermissions() {
    return this.allPermissions.filter(p => this.selectedValues[p.value] == true);
  }


  resetForm(replace = false) {

    if (!replace) {
      this.form.reset();
    }
    else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }


  newSignageCompilation() {
    this.isNewSignageCompilation = true;
    this.showValidationErrors = true;

    this.editingSignageCompilationName = null;
    this.selectedValues = {};
    this.signageCompilationEdit = new SignageCompilation();
    return this.signageCompilationEdit;
  }

  editSignageCompilation(signageCompilation: SignageCompilation) {
    if (signageCompilation) {
      this.isNewSignageCompilation = false;
      this.showValidationErrors = true;

      this.editingSignageCompilationName = signageCompilation.name;
      this.selectedValues = {};
      this.signageCompilationEdit = new SignageCompilation();
      Object.assign(this.signageCompilationEdit, signageCompilation);

      return this.signageCompilationEdit;
    }
    else {
      return this.newSignageCompilation();
    }
  }



  get canManageSignageCompilations() {
    return true;//this.accountService.userHasPermission(Permission.manageSignageCompilationsPermission)
  }
}
