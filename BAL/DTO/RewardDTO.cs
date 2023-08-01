using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO
{
    public class RewardDTO
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public double Balance { get; set; }

        public string UserName { get; set; }

        public byte[] ConcurrencyStamp { get; set; }

        public string Description { get; set; }
    }

    public class RewardOperationBaseDTO
    {
        public double Amount { get; set; }

        public string TransactionType { get; set; }

        public string Description { get; set; }
    }

    public class RewardOperationDTO: RewardOperationBaseDTO
    {
        public int RewardId { get; set; }

        public byte[] ConcurrencyStamp { get; set; }
    }

    public class RewardTransactionDTO : RewardOperationBaseDTO
    {
        public int Id { get; set; }

        public DateTime? TransactionDateTime { get; set; }
    }
}
