using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CBCenter.Models
{
    public class AddSchoolModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Enter School Name")]
        public string SchoolName { get; set; }

        [Required(ErrorMessage = "Enter School Address")]
        public string SchoolAddress { get; set; }
        public string GSTN { get; set; }

        [Required(ErrorMessage = "Enter Contact No")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string ContactNo { get; set; }
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string AlternateNo { get; set; }
    }
}