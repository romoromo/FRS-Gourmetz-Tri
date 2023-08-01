using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO
{
    public class WalletDTO
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public double Balance { get; set; }

        public byte[] ConcurrencyStamp { get; set; }

        public string UserName { get; set; }

        public string Description { get; set; }
    }

    public class WalletTopUpDTO
    {
        public int WalletId { get; set; }

        public double Amount { get; set; }

        public byte[] ConcurrencyStamp { get; set; }
    }

    public class WalletOperationBaseDTO
    {
        public double Amount { get; set; }

        public string TransactionType { get; set; }

        public string Description { get; set; }
    }

    public class WalletOperationDTO: WalletOperationBaseDTO
    {
        public int WalletId { get; set; }

        public byte[] ConcurrencyStamp { get; set; }
    }

    public class WalletTransactionDTO : WalletOperationBaseDTO
    {
        public int Id { get; set; }

        public DateTime? TransactionDateTime { get; set; }
    }
}
