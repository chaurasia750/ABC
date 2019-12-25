using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CBCenter.Models
{
    public class ReceivePaymentModel
    {
        public int SchoolsId { get; set; }
        
        [Required]
        public decimal? Amount { get; set; }
        public string ReceiveDate { get; set; }

        [Required(ErrorMessage ="Select Payment Mode")]
        [Range(1,4 ,ErrorMessage = "Select Payment Mode")]
        public Mode PaymentMode { get; set; }
    }

    public enum Mode
    {
        Cash=1,
        Check=2,
        ByAccount=3
    }
}