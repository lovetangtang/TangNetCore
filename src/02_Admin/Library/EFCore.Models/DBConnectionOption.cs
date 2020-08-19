using System;
using System.Collections.Generic;
using System.Text;

namespace EFCore.Models.Models
{
    public class DBConnectionOption
    {
        public string WriteConnection { get; set; }
        public List<string> ReadConnectionList { get; set; }
    }
}
