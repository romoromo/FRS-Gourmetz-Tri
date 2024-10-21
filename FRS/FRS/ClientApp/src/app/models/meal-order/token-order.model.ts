import { StudentGroup } from './student-group.model'

export class TokenOrder {

  constructor(id?: string) {

    this.id = id;
  }

  public id: string;
  public transactionTime: Date;
  public schoolId: string;
  public orderProfile: string;
  public profileId: string;
  public deliveryDate: Date;
  public periodId: string;
  public totalAmount: number;
  public paymentTime: Date;
  public totalPayment: number;
  public status: string;
  public remarks: string;
  public tokens: TokenOrdered[];
  public studentGroupId: string;
  public studentGroup: StudentGroup;
  public mealSessionDetailId: string;
  public storeId: string;
  public profileName: string;
  public periodName: string;
  public paymentTypeName: string;
  public selected: boolean;
  public bentoCode: string;
  public collectionTime?: Date;
  public returnTime?: Date;
  public voucherCode: string;
  public fomoId: string;
  public paymentNumber: string;
  public invoiceNumber: string;
  public mealDescription: string;
  public mealSessionDetailName: string;
}

export class TokenOrdered {
  public id: string;
  public tokenId: string;
  public mealTypeId: string;
  public tokenDesc: string;
  public qty: number;
  public orderId: string;
  public price: number;
  public subtotal: number;
  public groupQty: number;
  public selectedDishes: TokenOrderDish[];
  public tokenAltDishes: TokenAltDish[];
  public selectedCombinedDishes: TokenOrderCombinedDish[];
  public listCombinedDishes: any[];
}

export class TokenOrderDish {
  public id: string;
  public tokenOrderedId: string;
  public dishId: string;
  public qty: number;
  public dishLabel: string;
}

export class TokenOrderCombinedDish {
  public id: string;
  public tokenOrderedId: string;
  public cDishLabel: string;
  public cDishCode: string;
  public qty: number;
  public menuQty: number;
}

export class TokenAltDish {
  public id: string;
  public tokenOrderedId: string;
  public dishId: string;
}

export class PackingAllocation {
  public id: string;
  public packingDate: Date;
  public routeId: string;
  public outletId: string;
  public allocations: MealAllocation[];
  public dishes: DishAllocation[];
}

export class DishAllocation {
  public id: string;
  public packingId: string;
  public dishId: string;
  public qty: number;
}

export class MealAllocation {
  public id: string;
  public deliveryDate: Date;
  public mealSessionId: string;
  public outletId: string;
  public kitchenId: string;
  public timePacked: Date;
  public tokens: TokenLabel[];
}

export class TokenLabel {
  public id: string;
  public meal_allocation_id: string;
  public token_id: string;
  public token_name: string;
  public qty: number;
  public qty_dishes: number;
  public qty_pdishes: number;
  public qty_tdishes: number;
  public qty_menus: number;
  public deliveryDate: string;
  public deliveryDate2: Date;
  public timePacked: Date;
  public dishes: TokenDishLabel[];
}

export class TokenDishLabel {
  public id: string;
  public token_lable_id: string;
  public token_id: string;
  public token_name: string;
  public dish_id: string;
  public dish_name: string;
  public dish_code: string;
  public o_qty: number;
  public p_qty: number;
  public a_qty: number;
  public t_qty: number;
}

export class NewOrder {
  public transactionTime: Date;
  public profileId: string;
  public deliveryDate: any;
  public periodId: string;
  public studentName: string;
  public studentEmail: string;
  public quantity: number;
  public totalAmount: number;
  public remarks: string;
  public tokens: NewOrderToken[];
  public mealSessionDetailId: string;
  public mealSessionId: string;
  public createdBy?: string;
  public processedBy: string;
  public storeId: string;
  public outletId: string;
}

export class NewOrderToken {
  public tokenId: string;
  public mealTypeId: string;
  public tokenDesc: string;
  public qty: number;
  public dishId: string;
}

export class AmendOrder extends NewOrder {
  public voucherCode: string;
  public fomoId: string;
  public paymentNumber: string;
  public invoiceNumber: string;
  public paymentTypeName: string;
  public tokenOrderId: string;
  public mealSessionDetailName: string;
  public dishId: string;
  public updatedBy?: string;
  public reason: string;
  public status: string;
}
