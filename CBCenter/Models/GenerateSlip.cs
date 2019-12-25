using CBCenter.CBCenterBL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CBCenter.Models
{
    public class GenerateSlip
    {
        private static iTextSharp.text.Font fOrderSummary = FontFactory.GetFont("Calibri", 14f, 1, BaseColor.WHITE);
        private static iTextSharp.text.Font fPaybleAmount = FontFactory.GetFont("Arial", 14f, 1, BaseColor.BLACK);
        private static iTextSharp.text.Font fColHeder = FontFactory.GetFont("Arial", 8f, 1, BaseColor.BLACK);
        private static iTextSharp.text.Font fColValue = FontFactory.GetFont("Arial", 8f, 1, BaseColor.BLACK);
        private static float[] Mwidths = new float[3]
        {
      10f,
      300f,
      10f
        };
        private static float[] widths = new float[8]
        {
      34f,
      280f,
      30f,
      60f,
      70f,
      52f,
      52f,
      60f
        };

        public static string CreateSlip(string PdfName, string path, string billNo)
        {
            PdfName += ".pdf";
            try
            {
                PrintOrderSummary summary = GenerateSlip.PrintOrderSummary(billNo);
                using (Document document = new Document(PageSize.A4, 10f, 10f, 180f, 2f))
                {
                    PdfWriter.GetInstance(document, (Stream)new FileStream(path + PdfName, FileMode.Create)).PageEvent = (IPdfPageEvent)new PdfHeaderFooterEvent(summary);
                    document.Open();
                    PdfPTable Maintable = GenerateSlip.GetMainTable();
                    PdfPCell cell1 = new PdfPCell();
                    GenerateSlip.MainCellDraw(Maintable, cell1);
                    PdfPTable table1 = GenerateSlip.GetColumnName();
                    int num1 = 0;
                    foreach (BookCalculation bookCalculation in summary.BookCalculation)
                    {
                        if (num1 > 1 && num1 % 10 == 0)
                        {
                            Maintable.AddCell(new PdfPCell(table1)
                            {
                                Colspan = 1,
                                PaddingTop = 10f
                            });
                            PdfPCell cell2 = new PdfPCell();
                            cell2.Colspan = 1;
                            cell2.BorderWidthLeft = 0.0f;
                            cell2.PaddingTop = 10f;
                            cell2.BorderWidthTop = 0.0f;
                            Maintable.AddCell(cell2);
                            document.Add((IElement)Maintable);
                            document.NewPage();
                            Maintable = new PdfPTable(GenerateSlip.Mwidths);
                            Maintable = GenerateSlip.GetMainTable();
                            PdfPCell cell3 = new PdfPCell();
                            GenerateSlip.MainCellDraw(Maintable, cell3);
                            table1 = new PdfPTable(GenerateSlip.widths);
                            table1 = GenerateSlip.GetColumnName();
                        }
                        ++num1;
                        GenerateSlip.AddCell(table1, num1.ToString(), 1, GenerateSlip.fColValue, 1, true);
                        GenerateSlip.AddCell(table1, bookCalculation.BookName, 1, GenerateSlip.fColValue, 1, true);
                        GenerateSlip.AddCell(table1, bookCalculation.Quantity.ToString(), 1, GenerateSlip.fColValue, 1, true);
                        PdfPTable table2 = table1;
                        Decimal num2 = bookCalculation.Price;
                        string text1 = num2.ToString("#.00");
                        iTextSharp.text.Font fColValue1 = GenerateSlip.fColValue;
                        GenerateSlip.AddCell(table2, text1, 1, fColValue1, 1, true);
                        PdfPTable table3 = table1;
                        num2 = bookCalculation.Price * (Decimal)bookCalculation.Quantity;
                        string text2 = num2.ToString("#.00");
                        iTextSharp.text.Font fColValue2 = GenerateSlip.fColValue;
                        GenerateSlip.AddCell(table3, text2, 1, fColValue2, 1, true);
                        GenerateSlip.AddCell(table1, bookCalculation.Discount, 1, GenerateSlip.fColValue, 1, true);
                        PdfPTable table4 = table1;
                        num2 = bookCalculation.TotalDiscount;
                        string text3 = num2.ToString("#.00");
                        iTextSharp.text.Font fColValue3 = GenerateSlip.fColValue;
                        GenerateSlip.AddCell(table4, text3, 1, fColValue3, 1, true);
                        PdfPTable table5 = table1;
                        num2 = bookCalculation.NetAmount.Value;
                        string text4 = num2.ToString("#.00");
                        iTextSharp.text.Font fColValue4 = GenerateSlip.fColValue;
                        GenerateSlip.AddCell(table5, text4, 1, fColValue4, 1, true);
                    }
                    GenerateSlip.AddCell(table1, "Total", 2, GenerateSlip.fColHeder, 2, true);
                    GenerateSlip.AddCell(table1, summary.BookCalculation.Sum<BookCalculation>((Func<BookCalculation, int>)(x => x.Quantity)).ToString(), 2, GenerateSlip.fColHeder, 1, true);
                    GenerateSlip.AddCell(table1, summary.BookCalculation.Sum<BookCalculation>((Func<BookCalculation, Decimal>)(x => x.Price * (Decimal)x.Quantity)).ToString("#.00"), 2, GenerateSlip.fColHeder, 2, true);
                    AddCell(table1, summary.BookCalculation.Sum<BookCalculation>(x => x.NetAmount.Value).ToString("c2"), 1, fPaybleAmount, 4, true);
                    
                    GenerateSlip.AddCell(table1, summary.BookCalculation.Sum<BookCalculation>((Func<BookCalculation, Decimal>)(x => x.NetAmount.Value)).ToString("#.00"), 2, GenerateSlip.fColHeder, 1, true);
                    GenerateSlip.AddCell(table1, "Payable Amount", 1, GenerateSlip.fPaybleAmount, 4, true);
                    GenerateSlip.AddCell(table1, summary.BookCalculation.Sum<BookCalculation>((Func<BookCalculation, Decimal>)(x => x.NetAmount.Value)).ToString("c2"), 1, GenerateSlip.fPaybleAmount, 4, true);
                    Maintable.AddCell(new PdfPCell(table1)
                    {
                        Colspan = 1,
                        PaddingTop = 10f
                    });
                    PdfPCell cell4 = new PdfPCell();
                    cell4.Colspan = 1;
                    cell4.BorderWidthLeft = 0.0f;
                    cell4.PaddingTop = 10f;
                    cell4.BorderWidthTop = 0.0f;
                    Maintable.AddCell(cell4);
                    document.Add((IElement)Maintable);
                    document.Close();
                }
            }
            catch (Exception ex)
            {
            }
            return PdfName;
        }

        private static PdfPTable GetMainTable()
        {
            PdfPTable pdfPtable = new PdfPTable(3);
            pdfPtable.WidthPercentage = 95f;
            pdfPtable.SetWidths(GenerateSlip.Mwidths);
            pdfPtable.HorizontalAlignment = 1;
            pdfPtable.LockedWidth = false;
            return pdfPtable;
        }

        private static PdfPCell MainCellDraw(PdfPTable Maintable, PdfPCell cell)
        {
            cell = new PdfPCell(new Phrase("Order Summary", GenerateSlip.fOrderSummary));
            cell.Colspan = 3;
            cell.FixedHeight = 20f;
            cell.BackgroundColor = new BaseColor(9, 95, 89);
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            Maintable.AddCell(cell);
            cell = new PdfPCell();
            cell.Colspan = 1;
            cell.BorderWidthTop = 0.0f;
            cell.BorderWidthRight = 0.0f;
            cell.PaddingTop = 10f;
            Maintable.AddCell(cell);
            return cell;
        }

        protected static void AddCell(
          PdfPTable table,
          string text,
          int rowspan,
          iTextSharp.text.Font font,
          int colspan,
          bool isWidth)
        {
            table.AddCell(new PdfPCell(new Phrase(text, font))
            {
                MinimumHeight = !isWidth ? 20f : 40f,
                Rowspan = rowspan,
                Colspan = colspan,
                HorizontalAlignment = 1,
                VerticalAlignment = 5
            });
        }

        private static PdfPTable GetColumnName()
        {
            PdfPTable table = new PdfPTable(8);
            table.SetWidths(GenerateSlip.widths);
            table.WidthPercentage = 50f;
            table.HorizontalAlignment = 1;
            GenerateSlip.AddCell(table, "S.No.", 2, GenerateSlip.fColHeder, 1, false);
            GenerateSlip.AddCell(table, "Name of the Book", 2, GenerateSlip.fColHeder, 1, false);
            GenerateSlip.AddCell(table, "Qty.", 2, GenerateSlip.fColHeder, 1, false);
            GenerateSlip.AddCell(table, "Price", 2, GenerateSlip.fColHeder, 1, false);
            GenerateSlip.AddCell(table, "Gross Amt.", 2, GenerateSlip.fColHeder, 1, false);
            GenerateSlip.AddCell(table, "Discount (%)", 1, GenerateSlip.fColHeder, 2, false);
            GenerateSlip.AddCell(table, "Net Amt.", 2, GenerateSlip.fColHeder, 1, false);
            GenerateSlip.AddCell(table, "Discount", 1, GenerateSlip.fColHeder, 1, false);
            GenerateSlip.AddCell(table, "Total", 1, GenerateSlip.fColHeder, 1, false);
            return table;
        }

        public static PrintOrderSummary PrintOrderSummary(string BillNo)
        {
            var result = new PrintOrderSummary();
            try
            {
                using (CBCenterDBEntities dbentities = new CBCenterDBEntities())
                {
                    result = dbentities.BillTransactions.Join(dbentities.SchoolsMasters, x => x.SchoolsId, y => y.Id, (x, y) => new { school = y, order = x }).
                                 Where(x => x.order.BillNo == BillNo).Select(x => new PrintOrderSummary
                                 {
                                     CustomerAddress = x.school.SchoolAddress,
                                     CustomerName = x.school.SchoolName,
                                     OrderDate = x.order.OrderDate.Value,
                                     BillNo = x.order.BillNo,
                                     TransID = x.order.TransactionID,
                                     ContactNo = x.school.ContactNo
                                 }).SingleOrDefault();
                    result.BookCalculation = dbentities.BookTransactions.Where(x => x.TransactionID == result.TransID).Join(dbentities.BookMasters, x => x.BookID, y => y.BookId, (x, y)
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
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}