import { Component, Inject, OnInit } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material";

@Component({
  selector: "invoice-detail",
  templateUrl: "./invoice-detail.components.html",
  styleUrls: ["./invoice-detail.components.css"],
})
export class InvoiceDetailComponent implements OnInit {
  
  invoiceData: any = {};
  details: any[] = [];

  constructor(
    public dialogRef: MatDialogRef<InvoiceDetailComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ){  }

  ngOnInit() {
    if (this.data && this.data.data) {
      this.invoiceData = this.data.data.data.sales || {};
      this.details = this.data.data.data.salesItems || [];
    }
  }

  close() {
    this.dialogRef.close();
  }
}
