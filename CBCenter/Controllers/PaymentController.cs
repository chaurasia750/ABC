using CBCenter.CBCenterBL;
using CBCenter.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CBCenter.Controllers
{
    public class PaymentController : Controller
    {
        private CBCenterDBManager cBCenterDBManager;
        public PaymentController()
        {
            cBCenterDBManager = new CBCenterDBManager();
        }

        [HttpGet]
        public ActionResult Invoice()
        {
            
            try
            {
                
                ViewBag.Schools = cBCenterDBManager.GetSchoolList();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
            
            return View();
        }

        [HttpGet]
        public ActionResult ReceivePayment(int schoolsId)
        {
            ReceivePaymentModel model = new ReceivePaymentModel();
            model.SchoolsId = schoolsId;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult ReceivePayment(ReceivePaymentModel model)
        {
            if (ModelState.IsValid)
            {
             bool isSuccess=   cBCenterDBManager.AddPayment(model);
                if (isSuccess)
                {
                    return Json(new { success = "1" });
                }
            }
            return Json(new { success="0" });
        }
    }
}