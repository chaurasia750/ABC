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
    
    public partial class SchoolMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SchoolMaster()
        {
            this.BillTransactions = new HashSet<BillTransaction>();
        }
    
        public int SchoolID { get; set; }
        public string SchoolName { get; set; }
        public string SchoolAddress { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BillTransaction> BillTransactions { get; set; }
    }
}
