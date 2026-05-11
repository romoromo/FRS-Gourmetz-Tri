export class FASMonthlyBillingReportModel
{
    public id: number;
    public studentID: number;
    public studentName: string;
    public classlevelID: number;
    public classLevel: string;
    public classID: number;
    public class: string;
    public fasStudent: boolean;
    public deliveryDate: Date;
    public mealType:string;
    public mealName:string;
    public qty: number;
    public price: number;
    public invoiceNumber: string;
    public posInvoiceNumber: string;
    public amount: number;
    public collectionTime: Date;
	public studentStatus: string;
}