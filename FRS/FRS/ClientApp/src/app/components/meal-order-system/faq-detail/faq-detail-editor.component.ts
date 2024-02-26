import { Component, ViewChild, Inject, OnInit, AfterViewInit } from '@angular/core';

import { AlertService, MessageSeverity } from '../../../services/alert.service';
import { AccountService } from "../../../services/account.service";
import { Permission } from '../../../models/permission.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MealService } from 'src/app/services/meal-order/meal.service';
import { Filter } from 'src/app/models/sieve-filter.model';
import { FaqDetail } from 'src/app/models/meal-order/faq-subject.model';
import { FaqService } from 'src/app/services/meal-order/faq.service';
import { Subscription } from 'rxjs';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';

//declare var $;

@Component({
  selector: 'faq-detail-editor',
  templateUrl: './faq-detail-editor.component.html',
  styleUrls: ['./faq-detail-editor.component.css']
})


export class FaqDetailEditorComponent implements OnInit, AfterViewInit {
  private subscription: Subscription = new Subscription();
  private isNewFaqDetail = false;
  private isSaving: boolean;
  private showValidationErrors: boolean = true;
  private editingFaqDetailCode: string;
  private faqDetailEdit: FaqDetail = new FaqDetail();
  private allPermissions: Permission[] = [];
  private selectedValues: { [key: string]: boolean; } = {};
  public formResetToggle = true;
  public catererId: string;
  public changesSavedCallback: () => void;
  public changesFailedCallback: () => void;
  public changesCancelledCallback: () => void;
  public faqSubjects = [];

  public fg: FormGroup = new FormGroup({
    html: new FormControl('', Validators.required)
  });

  @ViewChild('f')
  private form;

  constructor(private alertService: AlertService, private faqService: FaqService, private accountService: AccountService,
    public dialogRef: MatDialogRef<FaqDetailEditorComponent>, private mealService: MealService, private sanitizer: DomSanitizer,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    if (typeof (data.faqDetail) != typeof (undefined)) {
      this.catererId = data.catererId;
      if (data.faqDetail.id) {
        this.editFaqDetail(data.faqDetail);
      } else {
        this.newFaqDetail();
      }
    }
    this.getFaqSubjects();
  }

  public editorOptions = {
    theme: 'snow',
    modules: {
      toolbar: {
        container:
          [
            ['bold', 'italic', 'underline', 'strike'],        // toggled buttons
            ['blockquote', 'code-block'],

            [{ 'header': 1 }, { 'header': 2 }],               // custom button values
            [{ 'list': 'ordered' }, { 'list': 'bullet' }],
            [{ 'script': 'sub' }, { 'script': 'super' }],      // superscript/subscript
            [{ 'indent': '-1' }, { 'indent': '+1' }],          // outdent/indent
            [{ 'direction': 'rtl' }],                         // text direction

            [{ 'size': ['small', false, 'large', 'huge'] }],  // custom dropdown
            [{ 'header': [1, 2, 3, 4, 5, 6, false] }],

            [{ 'color': [] }, { 'background': [] }],          // dropdown with defaults from theme
            [{ 'font': [] }],
            [{ 'align': [] }],

            ['clean']                                    // remove formatting button
 
          ]
      }
    }
  };

  //quillConfig = {
  //  toolbar: {
  //    container: [
  //      //['bold', 'italic', 'underline'],  // Basic formatting buttons
  //      //['link'],                          // Link button
  //      //['clean']                          // Remove formatting button

  //      ['bold', 'italic', 'underline', 'strike'],        // toggled buttons
  //      ['blockquote', 'code-block'],

  //      [{ 'header': 1 }, { 'header': 2 }],               // custom button values
  //      [{ 'list': 'ordered' }, { 'list': 'bullet' }],
  //      [{ 'script': 'sub' }, { 'script': 'super' }],      // superscript/subscript
  //      [{ 'indent': '-1' }, { 'indent': '+1' }],          // outdent/indent
  //      //[{ 'direction': 'rtl' }],                         // text direction

  //      [{ 'size': ['small', false, 'large', 'huge'] }],  // custom dropdown
  //      [{ 'header': [1, 2, 3, 4, 5, 6, false] }],

  //      [{ 'color': [] }, { 'background': [] }],          // dropdown with defaults from theme
  //      [{ 'font': [] }],
  //      [{ 'align': [] }],

  //      ['clean'],                                         // remove formatting button

  //      ['link']
  //    ]
  //  }
  //};

  public config = {
    placeholder: '',
    tabsize: 2,
    height: '200px',
    toolbar: [
      //['misc', ['codeview', 'undo', 'redo']],
      //['style', ['bold', 'italic', 'underline', 'clear']],
      ['font', ['bold', 'italic', 'underline', 'strikethrough', 'superscript', 'subscript', 'clear']],
      ['fontsize', ['fontname', 'fontsize']],
      ['para', ['ul', 'ol', 'paragraph', 'height']],
      ['insert', ['link']]
    ],
    fontNames: ['Helvetica', 'Arial', 'Arial Black', 'Comic Sans MS', 'Courier New', 'Roboto', 'Times']
  };

  ngOnInit() {
    

    //$('#summernote').summernote({
    //  placeholder: 'Type your text here...', // Placeholder text
    //  height: 300, // Height of the editor
    //  callbacks: {
    //    onChange: (contents) => {
    //      console.log('Editor content 2:', contents);
    //      this.faqDetailEdit.description = contents; // Update editorContent when the content changes
    //    }
    //  },
    //  toolbar: [
    //    // [groupName, [list of button]]
    //    ['style', ['bold', 'italic', 'underline', 'clear']],
    //    ['font', ['strikethrough', 'superscript', 'subscript']],
    //    ['fontsize', ['fontsize']],
    //    ['color', ['color']],
    //    ['para', ['ul', 'ol', 'paragraph']],
    //    ['height', ['height']]
    //  ]
    //});

  }

  ngAfterViewInit() {
    
  }

  public onBlur() {
    console.log('Blur');
  }

  get sanitizedHtml() {
    return this.sanitizer.bypassSecurityTrustHtml(this.fg.get('html').value);
  }

  onContentChange(content: string) {
    console.log('Editor content:', content);
    this.faqDetailEdit.description = content;
  }

  getFaqSubjects() {
    let filter = new Filter();
    filter.filters = '(IsActive)==true';
    this.subscription.add(this.faqService.getFaqSubjectsByFilter(filter)
      .subscribe(results => {
        this.faqSubjects = results.pagedData;
      },
        error => {
          //this.alertService.showStickyMessage("Get Error", `An error occured while retrieving locations.\r\nError: "${Utilities.getHttpResponseMessage(error)}"`,
          this.alertService.showStickyMessage("Get Error", `An error occured while retrieving records.\r\n"`,
            MessageSeverity.error);
        }));
  }

  private showErrorAlert(caption: string, message: string) {
    this.alertService.showMessage(caption, message, MessageSeverity.error);
  }


  private save() {
    this.isSaving = true;
    this.alertService.startLoadingMessage("Saving changes...");
    
    //this.faqDetailEdit.description = this.fg.get('html').value;
    if (this.isNewFaqDetail) {
      this.faqService.newFaqDetail(this.faqDetailEdit).subscribe(faqDetail => this.saveSuccessHelper(faqDetail), error => this.saveFailedHelper(error));
    }
    else {
      this.faqService.updateFaqDetail(this.faqDetailEdit).subscribe(response => this.saveSuccessHelper(), error => this.saveFailedHelper(error));
    }
  }


  private saveSuccessHelper(faqDetail?: FaqDetail) {
    if (faqDetail)
      Object.assign(this.faqDetailEdit, faqDetail);

    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.showValidationErrors = false;

    if (this.isNewFaqDetail)
      this.alertService.showMessage("Success", `\"${this.faqDetailEdit.label}\" was created successfully`, MessageSeverity.success);
    else
      this.alertService.showMessage("Success", `Changes to \"${this.faqDetailEdit.label}\" was saved successfully`, MessageSeverity.success);


    this.faqDetailEdit = new FaqDetail();
    this.resetForm();


    //if (!this.isNewFaqDetail && this.accountService.currentUser.facilities.some(r => r == this.editingFaqDetailCode))
    //    this.refreshLoggedInUser();

    if (this.changesSavedCallback)
      this.changesSavedCallback();

    this.dialogRef.close();
  }


  private saveFailedHelper(error: any) {
    this.isSaving = false;
    this.alertService.stopLoadingMessage();
    this.alertService.showStickyMessage("Save Error", "The below errors occured while saving your changes:", MessageSeverity.error);
    this.alertService.showStickyMessage(error, null, MessageSeverity.error);

    if (this.changesFailedCallback)
      this.changesFailedCallback();
  }


  private cancel() {
    this.faqDetailEdit = new FaqDetail();

    this.showValidationErrors = false;
    this.resetForm();

    this.alertService.resetStickyMessage();

    if (this.changesCancelledCallback)
      this.changesCancelledCallback();

    this.dialogRef.close();
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


  newFaqDetail() {
    this.isNewFaqDetail = true;
    this.showValidationErrors = true;

    this.editingFaqDetailCode = null;
    this.selectedValues = {};
    this.faqDetailEdit = new FaqDetail();
    return this.faqDetailEdit;
  }

  editFaqDetail(faqDetail: FaqDetail) {
    if (faqDetail) {
      this.isNewFaqDetail = false;
      this.showValidationErrors = true;

      this.editingFaqDetailCode = faqDetail.label;
      this.selectedValues = {};
      this.faqDetailEdit = new FaqDetail();
      Object.assign(this.faqDetailEdit, faqDetail);

      return this.faqDetailEdit;
    }
    else {
      return this.newFaqDetail();
    }
  }



  get canManageFaqDetails() {
    return this.accountService.userHasPermission(Permission.manageMOSOrderMgtFaqDetailsPermission)
  }
}
