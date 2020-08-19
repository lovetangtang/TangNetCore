using System;
using System.Collections.Generic;

namespace EFCore.Models.Models
{
    public partial class CmNumberDetail
    {
        public int Id { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public int? SerialCycle { get; set; }
        public int? SerialLength { get; set; }
        public int? SerialStart { get; set; }
        public bool? SerialZero { get; set; }
        public int? Const { get; set; }
        public string Custom { get; set; }
    }
}
