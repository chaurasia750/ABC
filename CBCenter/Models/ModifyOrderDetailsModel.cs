using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CBCenter.Models
{
    public class ModifyOrderDetailsModel
    {
        public List<BookCalculation> OrderDetails { get; set; }
        public string SchoolName { get; set; }
        public string BillNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public int TransactionId { get; set; }

        public string SchoolAddress { get; set; }
    }
    
}