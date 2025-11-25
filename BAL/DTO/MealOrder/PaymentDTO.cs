using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class PaymentDTO
    {
        public int Id { get; set; }

        public string InvoiceNumber { get; set; }

        public List<TokenOrderDTO> TokenOrders { get; set; }

        public List<MealPlanOrderDTO> MealPlanOrders { get; set; }

        public decimal subtotal { get; set; }
        public decimal discount { get; set; }
        public decimal gst { get; set; }
        public decimal subdisctotal { get; set; }
        public decimal transactionFee { get; set; }
        public decimal fixedTransactionFee { get; set; }
        public decimal total { get; set; }

        public int? VoucherId { get; set; }

        public int? PaymentTypeId { get; set; }

        public string email { get; set; }

        public int? UserId { get; set; }

        public int? StudentId { get; set; }

        public string name { get; set; }

        public string PaymentNumber { get; set; }

        public string Status { get; set; }

        ///<summary>
        ///If payment is generated from stripe, this is stripe checkout.session.id.
        ///I dont rename the field because dont want to break existing api.
        ///</summary>
        public string fomoid { get; set; }
        public bool invoiceSent { get; set; }

        public string userName { get; set; }
        public string studentName { get; set; }

        public DateTime CreatedDate { get; set; }

        public string version { get; set; }
    }
}
