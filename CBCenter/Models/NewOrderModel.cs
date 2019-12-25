using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CBCenter.Models
{
    public class NewOrderModel
    {
        [Required]
        [Remote("IsBillNoAvailable", "Order", HttpMethod = "POST", ErrorMessage = "Bill No. already exists")]
        public int? BillNo { get; set; }
        [Required]
        
        public string OrderDate { get; set; }
        [Required]
        [Display(Name = "School")]
        public int SelectedSchoolId { get; set; }
        public IEnumerable<SelectListItem> Schools { get; set; }

    }
}