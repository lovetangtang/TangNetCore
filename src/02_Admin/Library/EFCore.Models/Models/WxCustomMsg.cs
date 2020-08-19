using System;
using System.Collections.Generic;

namespace EFCore.Models.Models
{
    public partial class WxCustomMsg
    {
        public int Id { get; set; }
        public string LoginId { get; set; }
        public string ModCode { get; set; }
        public int? DocId { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }
        public string Link { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Cdate { get; set; }
        public string PicUrl { get; set; }
    }
}
