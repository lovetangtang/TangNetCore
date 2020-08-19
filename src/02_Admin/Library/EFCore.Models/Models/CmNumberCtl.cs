using System;
using System.Collections.Generic;

namespace EFCore.Models.Models
{
    public partial class CmNumberCtl
    {
        public int Id { get; set; }
        public int? NumberId { get; set; }
        public int? RoleId { get; set; }
        public int? DepId { get; set; }
        public int? BranchId { get; set; }
        public int EmpId { get; set; }
    }
}
