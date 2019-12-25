using CBCenter.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CBCenter.CBCenterBL
{
    public class CBCenterDBManager
    {
        public List<SelectListItem> GetSchoolList()
        {
            List<SelectListItem> schoolsMasters = new List<SelectListItem>();
            try
            {
                using (var db = new CBCenterDBEntities())
                {
                    schoolsMasters = db.SchoolsMasters.OrderBy(x => x.SchoolName).
                        Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.SchoolName + "-" + x.SchoolAddress }).ToList();

                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
            return schoolsMasters;
        }

        public bool AddPayment(ReceivePaymentModel model)
        {
            bool isSuccess = false;
            try
            {
                using (var db = new CBCenterDBEntities())
                {
                    ReceivePayment receivePayment = new ReceivePayment
                    {
                        Amount = model.Amount,
                        Mode = (int)model.PaymentMode,
                        SchoolsId = model.SchoolsId,
                        ReceiveDate = Convert.ToDateTime(model.ReceiveDate),
                        SystemReciveDate = DateTime.Now
                    };
                    db.ReceivePayments.Add(receivePayment);
                    db.SaveChanges();
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {

              
            }
            
            return isSuccess;
        }
       
    }
}