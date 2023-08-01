
export class UserWallet {

  constructor() {
  }

  public id: string;
  public userId: string;
  public balance: number;
  public concurrencyStamp: string;
  public userName: string;
}


export class WalletOperation {

  constructor() {
  }

  public walletId: string;
  public amount: number;
  public concurrencyStamp: string;
  public transactionType: string;
  public description: string;

}

