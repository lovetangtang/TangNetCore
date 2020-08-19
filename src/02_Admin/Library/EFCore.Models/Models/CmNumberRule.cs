using System;
using System.Collections.Generic;

namespace EFCore.Models.Models
{
    public partial class CmNumberRule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? Cdate { get; set; }
        public int? UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
