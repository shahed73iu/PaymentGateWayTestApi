using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IBMSPaymentGateway.Models
{
    public class PaymentGatewayTransaction
    {
        //public int? Id { get; set; }
        //public int? TypeId { get; set; }
        //public int? InstituteId { get; set; }
        //public int? PaymentGatewayId { get; set; }

        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////
        /// </summary>
        public bool IsLive { get; set; }
        public int? TransId { get; set; } //
        public Decimal TotalAmount { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAdd1 { get; set; }
        public string CustomerAdd2 { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerPostCode { get; set; }
        public string CustomerCountry { get; set; }
        public string CustomerPhone { get; set; }
        public bool IsEnableEmi { get; set; }
        public Decimal? ConvertionRate { get; set; }
        //  public string CustomerState { get; set; }
        //public int UserInfoId { get; set; }
        //public int CurrencyId { get; set; }
        //public string Currency { get; set; } // 
        //public Decimal? Amount { get; set; } // 
        //public Decimal? ChargePercentage { get; set; }
        //public Decimal? Charges { get; set; }
        //public Decimal? RoundingCharge { get; set; }
        //public Decimal? ReceiveableAmount { get; set; }
        //public string PayStatus { get; set; }
        //public DateTime PaymentDate { get; set; }
        //public string ValidationId { get; set; }
        //public string CardType { get; set; }
        //public string CardNo { get; set; }
        //public string CardIssuer { get; set; }
        //public string CardBrand { get; set; }
        //public string CardIssueCountry { get; set; }
        //public string CardIssueCountryCode { get; set; }

        //public string BankTransactionId { get; set; }
        //public string VarifyStatus { get; set; }
        //public string VarifyKey { get; set; }
        //public string RiskTitle { get; set; }
        //public string RiskCode { get; set; }
        ////public string ExtParam1 { get; set; }
        ////public string ExtParam2 { get; set; }
        ////public string ExtParam3 { get; set; }
        ////public string ExtParam4 { get; set; }

        //public DateTime? PaymentInitioalizeTime { get; set; }
        //public DateTime PaymentCompliteTime { get; set; }
        //public string FileReason { get; set; }
        //public string PaymentProcessor { get; set; }
        //public string ErrorCode { get; set; }
        //public string ErrorTitle { get; set; }
        //public string IPAddress { get; set; }
        //public int EmailInstallment { get; set; }
        //public Decimal EmilAmount { get; set; } 
        //public Decimal DiscountAmount { get; set; }
        //public Decimal DiscountPercentage { get; set; }
        //public string DiscountRemarks { get; set; }
        //public bool? IsCompleted { get; set; }
        //public int FeesCollectionId { get; set; }
        //public int FeesBootId { get; set; }
        //public int FeesCollectionTypeId { get; set; }

        //public bool? IsActive { get; set; }
        //public bool DoTransaction { get; set; }
        //public int? AcasdamicBranchId { get; set; }
        //public string Discription { get; set; }
        //public bool IsSuccessFrombackend { get; set; }
    }
}
