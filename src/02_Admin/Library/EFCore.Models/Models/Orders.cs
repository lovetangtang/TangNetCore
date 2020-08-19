using System;
using System.Collections.Generic;

namespace EFCore.Models.Models
{
    public partial class Orders
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? OrderId { get; set; }
    }
}
