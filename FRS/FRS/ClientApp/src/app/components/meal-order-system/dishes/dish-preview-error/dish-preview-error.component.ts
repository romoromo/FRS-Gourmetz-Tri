import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';

@Component({
  selector: 'app-dish-preview-error',
  templateUrl: './dish-preview-error.component.html',
  styleUrls: ['./dish-preview-error.component.css']
})
export class DishPreviewErrorComponent {
  messageErrors:any[] = []

  constructor(
    public dialogRef: MatDialogRef<DishPreviewErrorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    if (data) {
      this.messageErrors = data.dataResponse.messages
    }
  }

  
}
