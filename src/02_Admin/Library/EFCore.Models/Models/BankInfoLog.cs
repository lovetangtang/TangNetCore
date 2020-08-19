using System;
using System.Collections.Generic;

namespace EFCore.Models.Models
{
    public partial class BankInfoLog
    {
        public int Id { get; set; }
        public int? EmpId { get; set; }
        public string Content { get; set; }
        public int? OperateType { get; set; }
        public DateTime? Cdate { get; set; }
        public int? Moudel { get; set; }
    }
}
