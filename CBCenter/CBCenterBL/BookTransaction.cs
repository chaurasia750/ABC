//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CBCenter.CBCenterBL
{
    using System;
    using System.Collections.Generic;
    
    public partial class BookTransaction
    {
        public int BookTrnsID { get; set; }
        public Nullable<int> TransactionID { get; set; }
        public Nullable<int> BookID { get; set; }
        public string Discount { get; set; }
        public Nullable<decimal> TotalDiscount { get; set; }
        public Nullable<int> Qty { get; set; }
    
        public virtual BillTransaction BillTransaction { get; set; }
        public virtual BookMaster BookMaster { get; set; }
    }
}
