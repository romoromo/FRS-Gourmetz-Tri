
export class UserReward {

  constructor() {
  }

  public id: string;
  public userId: string;
  public balance: number;
  public concurrencyStamp: string;
  public userName: string;
}


export class RewardOperation {

  constructor() {
  }

  public rewardId: string;
  public amount: number;
  public concurrencyStamp: string;
  public transactionType: string;
  public description: string;

}

