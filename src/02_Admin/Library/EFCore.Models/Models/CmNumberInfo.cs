using System;
using System.Collections.Generic;

namespace EFCore.Models.Models
{
    public partial class CmNumberInfo
    {
        public int Id { get; set; }
        public int RuleId { get; set; }
        public int NumberId { get; set; }
        public int TypeId { get; set; }
    }
}
