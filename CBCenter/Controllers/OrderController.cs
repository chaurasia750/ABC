using CBCenter.CBCenterBL;
using CBCenter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Rotativa;
using System.Threading.Tasks;
using System.IO;

namespace CBCenter.Controllers
{
    public class OrderController : Controller
    {
        private CBCenterDBEntities dbentities = null;
        public OrderController()
        {
            dbentities = new CBCenterDBEntities();
        }
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (dbentities != null)
            {
                dbentities.Dispose();
            }

            base.OnActionExecuted(filterContext);
        }
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult NewOrder()
        {
            GetAllBook();
            NewOrderModel newOrder = new NewOrderModel();
            newOrder.Schools = GetSchoolList();
            return View(newOrder);
        }
        private List<SelectListItem> GetSchoolList()
        {
            List<SelectListItem> schoolList = new List<SelectListItem>();
            try
            {
                using (CBCenterDBEntities dbentities = new CBCenterDBEntities())
                {
                    schoolList = dbentities.SchoolsMasters.OrderBy(x => x.SchoolName).
                        Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.SchoolName + "-" + x.SchoolAddress }).ToList();
                }
            }
            catch (Exception ex)
            {


            }

            return schoolList;
        }
        private void GetAllBook()
        {
            if (Session["BookList"] == null)
            {
                using (CBCenterDBEntities db = new CBCenterDBEntities())
                {
                    Session["BookList"] = dbentities.BookMasters.Where(x=>x.BookYearsId==2)
                        .Select(x => new BooksName { BookName = x.BookName, BookId = x.BookId }).ToList();
                }

            }
        }
        List<BooksName> BookList = new List<BooksName>();
        [HttpPost]
        public JsonResult AutoBookName(string Prefix)
        {
            Prefix = Prefix.ToLower();
            if (Session["BookList"] == null)
            {
                GetAllBook();
            }
            BookList = Session["BookList"] as List<BooksName>;
            var bookList = BookList.Where(s => s.BookName.ToLower().Contains(Prefix))
                .Select(x => new BooksName { BookName = x.BookName, BookId = x.BookId }).ToList();
            return Json(bookList.ToArray(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SavedBookEntry(NewBook book)
        {
            int success = 0;
            if (ModelState.IsValid)
            {
                string[] totalDisc = book.Discount.Split('+');
                decimal discount = 0;
                if (totalDisc.Length <= 1)
                {
                    discount = Convert.ToInt32(book.Discount);
                }
                else
                {
                    discount = (Convert.ToDecimal(totalDisc[0]) + Convert.ToDecimal(totalDisc[1])) - (Convert.ToDecimal(totalDisc[0]) * Convert.ToDecimal(totalDisc[1])) / 100;
                }
                BookCalculation calculation = null;
                var result = dbentities.BookMasters.SingleOrDefault(x => x.BookId == book.BookID);
                if (result != null)
                {
                    calculation = new BookCalculation()
                    {
                        BookID = result.BookId,
                        BookName = result.BookName,
                        Quantity = book.Quantity,
                        TotalDiscount = discount,
                        Discount = book.Discount,
                        GrossAmount = book.Quantity * result.Price,
                        NetAmount = (book.Quantity * result.Price) - (book.Quantity * result.Price) * discount / 100,
                        DiscountAmt = (book.Quantity * result.Price) - ((book.Quantity * result.Price) - (book.Quantity * result.Price) * discount / 100)
                    };
                    try
                    {
                        CreateXML(calculation);
                        success = 1;
                    }
                    catch (Exception ex)
                    {
                        success = 0;
                    }

                }
            }
            return Json(new { succes = success });
        }

        public ActionResult BookEntry()
        {
            return PartialView();
        }

        private IEnumerable<BookCalculation> ReadXMlEntry()
        {
            XDocument xmldoc = XDocument.Load(Server.MapPath("~/XMLFiles/" + Session["xmlFileName"].ToString()));
            var bind = xmldoc.Descendants("Book").Select(p => new
           BookCalculation
            {
                BookID = Convert.ToInt32(p.Element("BookID").Value),
                BookName = p.Element("BookName").Value,
                Quantity = Convert.ToInt32(p.Element("Quantity").Value),
                GrossAmount = Convert.ToDecimal(p.Element("GrossAmount").Value),
                Discount = (p.Element("Discount").Value),
                TotalDiscount = Convert.ToDecimal(p.Element("TotalDiscount").Value),
                DiscountAmt = Convert.ToDecimal(p.Element("DiscountAmt").Value),
                NetAmount = Convert.ToDecimal(p.Element("NetAmount").Value)
            });
            return bind;
        }

        private void CreateXML(BookCalculation calculation)
        {
            string xmlFileName = "";
            if (Session["xmlFileName"] == null)
            {
                xmlFileName = Guid.NewGuid() + ".xml";
                Session["xmlFileName"] = xmlFileName;
                XmlTextWriter writer = new XmlTextWriter(Server.MapPath("~/XMLFiles/" + xmlFileName), System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;
                //ROOT Element
                writer.WriteStartElement("Books");
                AddNode(calculation, writer);
                writer.Close();
            }
            else
            {
                XElement Books = new XElement("Book",
               new XElement("BookID", calculation.BookID),
               new XElement("BookName", calculation.BookName),
               new XElement("Quantity", calculation.Quantity),
               new XElement("GrossAmount", calculation.GrossAmount),
               new XElement("Discount", calculation.Discount),
               new XElement("TotalDiscount", calculation.TotalDiscount),
               new XElement("DiscountAmt", calculation.DiscountAmt),
               new XElement("NetAmount", calculation.NetAmount)
               );
                xmlFileName = Session["xmlFileName"].ToString();
                XDocument xmldoc = XDocument.Load(Server.MapPath("~/XMLFiles/" + xmlFileName));
                xmldoc.Root.Add(Books);
                xmldoc.Save(Server.MapPath("~/XMLFiles/" + xmlFileName));
            }
        }
        private void AddNode(BookCalculation cal, XmlTextWriter writer)
        {
            writer.WriteStartElement("Book");
            writer.WriteStartElement("BookID");
            writer.WriteString(cal.BookID.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("BookName");
            writer.WriteString(cal.BookName);
            writer.WriteEndElement();

            writer.WriteStartElement("Quantity");
            writer.WriteString(cal.Quantity.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("GrossAmount");
            writer.WriteString(cal.GrossAmount.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Discount");
            writer.WriteString(cal.Discount.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("TotalDiscount");
            writer.WriteString(cal.TotalDiscount.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("NetAmount");
            writer.WriteString(cal.NetAmount.ToString());
            writer.WriteEndElement();


            writer.WriteStartElement("DiscountAmt");
            writer.WriteString(cal.DiscountAmt.ToString());
            writer.WriteEndElement();
        }

        public ActionResult TempSellEntry()
        {
            List<BookCalculation> sellBookList = ReadXMlEntry().ToList();
            return PartialView("_TempSellBookList", sellBookList);
        }

        public ActionResult SaveRecords(SellCustomerRecords records)
        {
            int SchoolID = 0;
            int success = 0;
            if (ModelState.IsValid)
            {
                try
                {
                    records.BookCalculations = ReadXMlEntry().ToList();
                    SchoolMaster schoolMaster = new SchoolMaster
                    {
                        SchoolName = records.CustomerName,
                        SchoolAddress = records.CustomerAddress,
                        AddDate = DateTime.Now
                    };
                    var schoolRecords = dbentities.SchoolMasters.FirstOrDefault(x => x.SchoolName == records.CustomerName.Trim());
                    if (schoolRecords == null)
                    {
                        var saveSchoolRecords = dbentities.SchoolMasters.Add(schoolMaster);
                        dbentities.SaveChanges();
                        success++;
                        SchoolID = saveSchoolRecords.SchoolID;
                    }
                    else
                    {
                        SchoolID = schoolRecords.SchoolID;
                    }

                    BillTransaction bill = new BillTransaction
                    {
                        SchoolID = SchoolID,
                        BillNo = records.OrderNO,
                        OrderDate = Convert.ToDateTime(records.OrderDate),
                        SystemDate = System.DateTime.Now
                    };
                    var saveBillTransactions = dbentities.BillTransactions.Add(bill);
                    dbentities.SaveChanges();
                    success++;
                    foreach (var item in records.BookCalculations)
                    {
                        BookTransaction bookTransaction = new BookTransaction
                        {
                            TransactionID = saveBillTransactions.TransactionID,
                            BookID = item.BookID,
                            Discount = item.Discount,
                            TotalDiscount = item.TotalDiscount,
                            Qty = item.Quantity
                        };
                        dbentities.BookTransactions.Add(bookTransaction);
                        dbentities.SaveChanges();

                    }
                    Session.Remove("xmlFileName");
                }
                catch (Exception ex)
                {
                }

            }
            return Json(new { BillNo = records.OrderNO, success = success });
        }

        public async Task<ActionResult> SaveNewOrder(NewOrderModel newOrder)
        {
            NewOrderSummaryModel newOrderSummary = new NewOrderSummaryModel();
            int success = 0;
            if (ModelState.IsValid)
            {
                try
                {
                    newOrderSummary.BookCalculations = ReadXMlEntry().ToList();
                    newOrderSummary.NewOrder = newOrder;

                    BillTransaction billTransaction = new BillTransaction
                    {
                        BillNo = newOrder.BillNo.Value.ToString(),
                        SchoolsId = newOrder.SelectedSchoolId,
                        OrderDate = Convert.ToDateTime(newOrder.OrderDate),
                        SystemDate = DateTime.Now
                    };
                    using (CBCenterDBEntities db = new CBCenterDBEntities())
                    {
                        var addBills = db.BillTransactions.Add(billTransaction);
                        int billSuccess = await db.SaveChangesAsync();
                        success++;
                        if (billSuccess > 0)
                        {
                            var bookOrders = newOrderSummary.BookCalculations.Select(x => new BookTransaction
                            {
                                BookID = x.BookID,
                                TransactionID = addBills.TransactionID,
                                Discount = x.Discount,
                                Qty = x.Quantity,
                                TotalDiscount = x.TotalDiscount
                            });

                            var orderSave = db.BookTransactions.AddRange(bookOrders);
                            await db.SaveChangesAsync();
                            Session.Remove("xmlFileName");
                            success++;
                            return Json(new { transactionId = addBills.TransactionID, success = success });
                            //return RedirectToActionPermanent("ModifyOrderDetails", "Order", new { transactionId = addBills.TransactionID });
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return Json(new { BillNo = newOrder.BillNo, success = success });
        }

        [HttpPost]
        public JsonResult IsBillNoAvailable(int? BillNo)
        {
            string billNo = BillNo.Value.ToString();
            return Json(!dbentities.BillTransactions.Any(x => x.BillNo == billNo));

        }

        public ActionResult PrintOrder(string BillNo)
        {
            return View(PrintOrderSummary(BillNo));
        }

        public ActionResult SearchOrder()
        {
            return View();
        }
        public ActionResult GotoPrint(string BillNo)
        {
            return View(PrintOrderSummary(BillNo));
        }
        public ActionResult PrintView(string BillNo)
        {
            try
            {
                string path = Server.MapPath("~/TransactionSlip/");
                string pdfFullPath = GenerateSlip.CreateSlip(BillNo + "_" + DateTime.Now.Year.ToString(), path, BillNo);
                string fullPdfPath = Server.MapPath("~/TransactionSlip/" + pdfFullPath);
                FileStream fss = new FileStream(fullPdfPath, FileMode.Open);
                byte[] bytes = new byte[fss.Length];
                fss.Read(bytes, 0, Convert.ToInt32(fss.Length));
                fss.Close();
                System.IO.File.Delete(fullPdfPath);
                ModelState.Clear();
                return File(bytes, "application/pdf");
            }
            catch (Exception)
            {
            }
            return View();
        }

        private PrintOrderSummary PrintOrderSummary(string BillNo)
        {
            var result = new PrintOrderSummary();
            try
            {
                result = dbentities.BillTransactions.Join(dbentities.SchoolsMasters, x => x.SchoolsId, y => y.Id,
                    (x, y) => new { school = y, order = x }).
                             Where(x => x.order.BillNo == BillNo).Select(x => new PrintOrderSummary
                             {
                                 CustomerAddress = x.school.SchoolAddress,
                                 CustomerName = x.school.SchoolName,
                                 OrderDate = x.order.OrderDate.Value,
                                 BillNo = x.order.BillNo,
                                 TransID = x.order.TransactionID,
                                 ContactNo=x.school.ContactNo
                             }).SingleOrDefault();

                result.BookCalculation = dbentities.BookTransactions.Where(x => x.TransactionID == result.TransID)
                    .Join(dbentities.BookMasters, x => x.BookID, y => y.BookId, (x, y)
                        => new BookCalculation
                        {
                            Quantity = x.Qty.Value,
                            BookName = y.BookName,
                            Price = y.Price.Value,
                            TotalDiscount = x.TotalDiscount.Value,
                            GrossAmount = x.Qty.Value * y.Price.Value,
                            Discount = x.Discount,
                            BookID = y.BookId,
                            NetAmount = (x.Qty.Value * y.Price.Value) - (x.Qty.Value * y.Price.Value) * x.TotalDiscount.Value / 100,
                        }).ToList();
            }
            catch (Exception)
            {
            }
            return result;
        }
        [HttpPost]
        public ActionResult DeleteXMLEntry(int BookID)
        {
            if (Session["xmlFileName"] != null)
            {
                try
                {
                    string path = "~/XMLFiles/" + Session["xmlFileName"].ToString();
                    XDocument xmldoc = XDocument.Load(Server.MapPath(path));
                    XElement bookentry = xmldoc.Descendants("Book").
                        FirstOrDefault(p => p.Element("BookID").Value == BookID.ToString());
                    if (bookentry != null)
                    {
                        bookentry.Remove();
                        xmldoc.Save(Server.MapPath(path));
                        return Json(new { del = 1 });
                    }
                }
                catch (Exception ex)
                {


                }
            }

            return Json(new { del = 0 });
        }

        [HttpGet]
        public ActionResult PreviousOrder(int schoolId)
        {
            List<PreviousOrderDetails> previousOrderDetails = new List<PreviousOrderDetails>();
            try
            {
                using (CBCenterDBEntities db = new CBCenterDBEntities())
                {
                    previousOrderDetails = db.Sp_GetPreviousOrderDetails(schoolId).
                                                Select(x => new PreviousOrderDetails
                                                { BillNo = x.BillNo, TotalOrderPrice = x.Totalprice, OrderDate = x.OrderDate }).ToList();
                }
            }
            catch (Exception ex)
            {
            }

            return PartialView(previousOrderDetails);
        }

        public ActionResult ModifyOrderDetails(int transactionId)
        {
            ModifyOrderDetailsModel modifyOrderDetails = new ModifyOrderDetailsModel();
            try
            {
                using (var db = new CBCenterDBEntities())
                {
                    modifyOrderDetails = dbentities.BillTransactions.Where(x => x.TransactionID == transactionId).AsEnumerable()
                        .Join(db.SchoolsMasters, b => b.SchoolsId, s => s.Id,
                          (b, s) => new { b = b, s = s }).AsEnumerable()
                          .Select(x => new ModifyOrderDetailsModel
                          {
                              BillNo = x.b.BillNo,
                              OrderDate = x.b.OrderDate,
                              SchoolName = x.s.SchoolName,
                              TransactionId = x.b.TransactionID,
                              SchoolAddress = x.s.SchoolAddress
                          }).SingleOrDefault();

                    modifyOrderDetails.OrderDetails = OrderDetailsList(transactionId, db);
                }
            }
            catch (Exception ex)
            {

            }

            return View(modifyOrderDetails);
        }

        [HttpPost]
        public ActionResult UpdateOrder(UpdateOrderModel updateOrder)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string[] totalDisc = updateOrder.Discount.Split('+');
                    decimal discount = 0;
                    if (totalDisc.Length <= 1)
                    {
                        discount = Convert.ToInt32(updateOrder.Discount);
                    }
                    else
                    {
                        discount = (Convert.ToDecimal(totalDisc[0]) + Convert.ToDecimal(totalDisc[1])) - (Convert.ToDecimal(totalDisc[0]) * Convert.ToDecimal(totalDisc[1])) / 100;
                    }
                    using (dbentities = new CBCenterDBEntities())
                    {
                        var result = dbentities.BookTransactions.SingleOrDefault(x => x.BookTrnsID == updateOrder.BookTransactionId);
                        if (result != null)
                        {
                            result.Qty = updateOrder.Quantity;
                            result.Discount = updateOrder.Discount;
                            result.TotalDiscount = discount;
                            dbentities.SaveChanges();
                            return Json(new { success = "success", TransactionId = result.TransactionID });
                        }
                    }
                }
                catch (Exception ex)
                {

                    return Json(new { success = ex.Message });
                }
            }
            return Json(new { success = "UnKnown Error" });
        }

        private List<BookCalculation> OrderDetailsList(int transactionId, CBCenterDBEntities db)
        {
            List<BookCalculation> bookCalculations = new List<BookCalculation>();
            try
            {
                bookCalculations = db.BookTransactions.Where(x => x.TransactionID == transactionId).AsEnumerable()
                        .Join(db.BookMasters, bt => bt.BookID, bm => bm.BookId, (bt, bm) => new { bt = bt, bm = bm })
                        .Select(x => new BookCalculation
                        {
                            BookTransactionId = x.bt.BookTrnsID,
                            BookID = x.bt.BookID.Value,
                            BookName = x.bm.BookName,
                            Quantity = x.bt.Qty.Value,
                            Discount = x.bt.Discount,
                            Price = x.bm.Price.Value,
                            TotalDiscount = x.bt.TotalDiscount.Value,
                            GrossAmount = x.bt.Qty.Value * x.bm.Price.Value,
                            NetAmount = (x.bt.Qty.Value * x.bm.Price.Value) - (x.bt.Qty.Value * x.bm.Price.Value) * x.bt.TotalDiscount.Value / 100,
                            DiscountAmt = (x.bt.Qty.Value * x.bm.Price.Value) - ((x.bt.Qty.Value * x.bm.Price.Value) - (x.bt.Qty.Value * x.bm.Price.Value) * x.bt.TotalDiscount.Value / 100)

                        }).ToList();
            }
            catch (Exception ex)
            {


            }



            return bookCalculations;
        }

        public ActionResult OrderDetails(int transactionId)
        {
            List<BookCalculation> bookCalculations = new List<BookCalculation>();
            using (var db = new CBCenterDBEntities())
            {
                bookCalculations = OrderDetailsList(transactionId, db);

            }
            return PartialView("UpdateOrderDetails", bookCalculations);
        }

        [HttpPost]
        public ActionResult DeleteTransactionBook(int bookTransactionId)
        {
            try
            {
                using (var db = new CBCenterDBEntities())
                {
                    int tranasctionId = 0;
                    var result = db.BookTransactions.SingleOrDefault(x => x.BookTrnsID == bookTransactionId);
                    tranasctionId = result.TransactionID.Value;
                    db.BookTransactions.Remove(result);
                    db.SaveChanges();
                    return Json(new { success = "success", TransactionId = tranasctionId });
                }
            }
            catch (Exception ex)
            {


            }

            return Json(new { success = "UnKnown Error" });
        }
    }
}