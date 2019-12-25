using CBCenter.CBCenterBL;
using CBCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CBCenter.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var db = new CBCenterDBEntities())
            {
                ViewBag.TotalBook = db.BookMasters.Count(x => x.BookYearsId == 1);
                ViewBag.TotalSchools = db.SchoolsMasters.Count();
                ViewBag.TotalOrders = db.BillTransactions.Count();
            }
            return View();
        }

        public ActionResult RecentlyOrders()
        {
            List<PreviousOrderDetails> recentlyOrders = new List<PreviousOrderDetails>();
            using (var db = new CBCenterDBEntities())
            {
                recentlyOrders = db.Sp_GetPreviousOrderDetails(null).
                    Select(x => new PreviousOrderDetails
                    {
                        BillNo = x.BillNo,
                        OrderDate = x.OrderDate,
                        TotalOrderPrice = x.Totalprice
                    }).ToList();
            }
            return PartialView(recentlyOrders);
        }

        public ActionResult TotalOrders()
        {
            List<PreviousOrderDetails> allOrders = new List<PreviousOrderDetails>();
            using (var db = new CBCenterDBEntities())
            {
                allOrders = db.Sp_GetAllOrderDetails().
                    Select(x => new PreviousOrderDetails
                    {
                        BillNo = x.BillNo,
                        OrderDate = x.OrderDate,
                        TotalOrderPrice = x.Totalprice,
                        BillTransactionId = x.TransactionID
                    }).ToList();
            }
            return View(allOrders);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}