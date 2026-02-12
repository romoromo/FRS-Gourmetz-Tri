import { Component, Inject, OnInit } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material";

@Component({
  selector: "pos-detail",
  templateUrl: "./pos-detail.component.html",
  styleUrls: ["./pos-detail.component.css"],
})
export class PosDetailComponent implements OnInit {
  
  invoiceData: any = {};
  details: any[] = [];

  constructor(
    public dialogRef: MatDialogRef<PosDetailComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ){  }

  ngOnInit() {
    if (this.data && this.data.data) {
      this.invoiceData = this.data.data.data.sales || {};
      this.details = this.data.data.data.sales_items || [];
    }
  }

  close() {
    this.dialogRef.close();
  }
}
