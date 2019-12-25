using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CBCenter.Models
{
    public class NewBook
    {
        [Required(ErrorMessage ="Select Book Name")]
        public int BookID { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string Discount { get; set; }
        public string BookName { get; set; }
    }

    public class BookCalculation
    {
        public int BookID { get; set; }
        public string BookName { get; set; }
        public decimal? GrossAmount { get; set; }
        public decimal? NetAmount { get; set; }
        public string Discount { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal? DiscountAmt { get; set; }
        public int BookTransactionId { get; set; }
    }
    public class SellCustomerRecords
    {
        public List<BookCalculation> BookCalculations { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string CustomerAddress { get; set; }
        [Required]
        public string OrderNO { get; set; }
        [Required]
        public string OrderDate { get; set; }
    }
    public class PrintOrderSummary
    {
        public List<BookCalculation> BookCalculation { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string BillNo { get; set; }
        public DateTime OrderDate { get; set; }
        public int TransID { get; set; }
        public string ContactNo { get; set; }
    }

    public class NewOrderSummaryModel
    {
        public NewOrderModel NewOrder { get; set; }
        public List<BookCalculation> BookCalculations { get; set; }
    }

    public class UpdateOrderModel
    {
        [Required]
        public int BookTransactionId { get; set; }
        [Required]
        public int Quantity { get; set; }
        public string Discount { get; set; }
    }
}