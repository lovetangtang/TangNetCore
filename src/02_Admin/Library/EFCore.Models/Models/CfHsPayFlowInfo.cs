using System;
using System.Collections.Generic;

namespace EFCore.Models.Models
{
    public partial class CfHsPayFlowInfo
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string CmName { get; set; }
        public string SignCompany { get; set; }
        public string CmType { get; set; }
        public decimal? CmMoney { get; set; }
        public decimal? CmDynamicMoney { get; set; }
        public string BankAccount { get; set; }
        public decimal? BeforePayMoney { get; set; }
        public decimal? BeforePaidMoney { get; set; }
        public string CmTotalScale { get; set; }
        public decimal? CmPayMoney { get; set; }
        public decimal? CmUnpaidMoney { get; set; }
        public string CmMeetScale { get; set; }
        public string Payment { get; set; }
        public decimal? NowPayMoney { get; set; }
        public decimal? NowRealPayMoney { get; set; }
        public string CapMoney { get; set; }
        public DateTime? PlanDate { get; set; }
        public string MoneyName { get; set; }
        public string PayMode { get; set; }
        public int? CempId { get; set; }
        public DateTime? Cdate { get; set; }
        public string CempName { get; set; }
    }
}
