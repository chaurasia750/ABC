using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CBCenter.Models
{
    public class PreviousOrderDetails
    {
        public string BillNo { get; set; }
        public decimal? TotalOrderPrice { get; set; }
        public string OrderDate { get; set; }

        public string SchoolName { get; set; }
        public int BillTransactionId { get; set; }
    }
}