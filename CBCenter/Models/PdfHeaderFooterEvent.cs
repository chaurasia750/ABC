using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf.draw;
using CBCenter.CBCenterBL;

namespace CBCenter.Models
{
    public class PdfHeaderFooterEvent : PdfPageEventHelper
    {
        private iTextSharp.text.Font footerFont = FontFactory.GetFont("Calibri", 9f, 1, BaseColor.BLACK);
        private PrintOrderSummary Summary;
        private PdfTemplate total;

        public PdfHeaderFooterEvent(PrintOrderSummary summary)
        {
            this.Summary = summary;
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            base.OnOpenDocument(writer, document);
            this.total = writer.DirectContent.CreateTemplate(30f, 16f);
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
            iTextSharp.text.Font font1 = FontFactory.GetFont("Arial", 9f, 0);
            iTextSharp.text.Font font2 = FontFactory.GetFont("Arial", 15f, 1, BaseColor.BLACK);
            iTextSharp.text.Font font3 = FontFactory.GetFont("Arial", 9f, 1);
            iTextSharp.text.Font font4 = FontFactory.GetFont("Calibri", 12f, 0, BaseColor.BLACK);
            DottedLineSeparator dottedLineSeparator = new DottedLineSeparator();
            dottedLineSeparator.Offset = -3f;
            dottedLineSeparator.Gap = 3f;
            Rectangle pageSize = document.PageSize;
            string str = "Mob: 9430074984";
            PdfPTable pdfPtable = new PdfPTable(8);
            pdfPtable.TotalWidth = 600f;
            pdfPtable.HorizontalAlignment = 1;
            PdfPCell cell1 = new PdfPCell((Phrase)new Paragraph("GSTN -10CLJPK7738R1ZG", font1)
            {
                Alignment = 0
            });
            cell1.Border = 0;
            cell1.Colspan = 3;
            cell1.HorizontalAlignment = 0;
            pdfPtable.AddCell(cell1);
            PdfPCell cell2 = new PdfPCell((Phrase)new Paragraph("Cash/Credit Memo" + "\nGST EXEMPT PRINTED BOOKS", font1)
            {
                Alignment = 0
            });
            cell2.Border = 0;
            cell2.Colspan = 3;
            cell2.HorizontalAlignment = 0;
            pdfPtable.AddCell(cell2);
            PdfPCell cell3 = new PdfPCell(new Phrase(str, font1));
            cell3.Colspan = 2;
            cell3.Border = 0;
            cell3.PaddingRight = 30f;
            cell3.HorizontalAlignment = 2;
            pdfPtable.AddCell(cell3);
            PdfPCell cell4 = new PdfPCell(new Phrase("CHILDREN BOOK CENTER", font2));
            cell4.Border = 0;
            cell4.Colspan = 8;
            cell4.PaddingTop = 5f;
            cell4.HorizontalAlignment = 1;
            pdfPtable.AddCell(cell4);
            PdfPCell cell5 = new PdfPCell(new Phrase("GANDHI CHOWK, ARERAJ, EAST CHAMPARAN-845411", font1));
            cell5.Border = 0;
            cell5.Colspan = 8;
            cell5.HorizontalAlignment = 1;
            pdfPtable.AddCell(cell5);
            PdfPCell cell6 = new PdfPCell((Phrase)new Paragraph(new Chunk((IDrawInterface)new LineSeparator(0.0f, 100f, BaseColor.BLACK, 0, 1f))));
            cell6.Border = 0;
            cell6.Colspan = 8;
            cell6.PaddingTop = -5f;
            cell6.PaddingRight = 20f;
            pdfPtable.AddCell(cell6);
            PdfPCell cell7 = new PdfPCell(new Phrase("Bill No. :", font3));
            cell7.Colspan = 1;
            cell7.Border = 0;
            cell7.HorizontalAlignment = 2;
            pdfPtable.AddCell(cell7);
            Paragraph paragraph1 = new Paragraph();
            paragraph1.Add((IElement)new Phrase("       " + this.Summary.BillNo, font4));
            paragraph1.Add((IElement)dottedLineSeparator);
            PdfPCell cell8 = new PdfPCell((Phrase)paragraph1);
            cell8.Colspan = 3;
            cell8.Border = 0;
            cell8.HorizontalAlignment = 0;
            cell8.VerticalAlignment = 1;
            pdfPtable.AddCell(cell8);
            PdfPCell cell9 = new PdfPCell(new Phrase("Date :", font3));
            cell9.Colspan = 1;
            cell9.Border = 0;
            cell9.HorizontalAlignment = 2;
            pdfPtable.AddCell(cell9);
            Paragraph paragraph2 = new Paragraph();
            paragraph2.Add((IElement)new Phrase(this.Summary.OrderDate.ToString("dd MMM yyyy"), font4));
            paragraph2.Add((IElement)dottedLineSeparator);
            PdfPCell cell10 = new PdfPCell((Phrase)paragraph2);+
            cell10.Colspan = 3;
            cell10.PaddingRight = 30f;
            cell10.Border = 0;
            cell10.VerticalAlignment = 2;
            cell10.HorizontalAlignment = 1;
            pdfPtable.AddCell(cell10);
            PdfPCell cell11 = new PdfPCell(new Phrase("Name :", font3));
            cell11.Colspan = 1;
            cell11.Border = 0;
            cell11.PaddingTop = 10f;
            cell11.HorizontalAlignment = 2;
            pdfPtable.AddCell(cell11);
            Paragraph paragraph3 = new Paragraph();
            paragraph3.Add((IElement)new Phrase("       " + this.Summary.CustomerName, font4));
            paragraph3.Add((IElement)dottedLineSeparator);
            PdfPCell cell12 = new PdfPCell((Phrase)paragraph3);
            cell12.Colspan = 7;
            cell12.PaddingTop = 10f;
            cell12.PaddingRight = 30f;
            cell12.Border = 0;
            cell12.VerticalAlignment = 2;
            cell12.HorizontalAlignment = 1;
            pdfPtable.AddCell(cell12);
            PdfPCell cell13 = new PdfPCell(new Phrase("ContactNo. :", font3));
            cell13.Colspan = 1;
            cell13.Border = 0;
            cell13.PaddingTop = 10f;
            cell13.HorizontalAlignment = 2;
            pdfPtable.AddCell(cell13);
            Paragraph paragraph4 = new Paragraph();
            paragraph4.Add((IElement)new Phrase("       " + this.Summary.ContactNo, font4));
            paragraph4.Add((IElement)dottedLineSeparator);
            PdfPCell cell14 = new PdfPCell((Phrase)paragraph4);
            cell14.Colspan = 7;
            cell14.PaddingTop = 10f;
            cell14.PaddingRight = 30f;
            cell14.Border = 0;
            cell14.VerticalAlignment = 2;
            cell14.HorizontalAlignment = 1;
            pdfPtable.AddCell(cell14);
            PdfPCell cell15 = new PdfPCell(new Phrase("Address :", font3));
            cell15.Colspan = 1;
            cell15.PaddingTop = 10f;
            cell15.Border = 0;
            cell15.HorizontalAlignment = 2;
            pdfPtable.AddCell(cell15);
            Paragraph paragraph5 = new Paragraph();
            paragraph5.Add((IElement)new Phrase("       " + this.Summary.CustomerAddress, font4));
            paragraph5.Add((IElement)dottedLineSeparator);
            PdfPCell cell16 = new PdfPCell((Phrase)paragraph5);
            cell16.Colspan = 7;
            cell16.PaddingTop = 10f;
            cell16.PaddingRight = 30f;
            cell16.Border = 0;
            cell16.VerticalAlignment = 2;
            cell16.HorizontalAlignment = 1;
            pdfPtable.AddCell(cell16);
            PdfPCell cell17 = new PdfPCell((Phrase)new Paragraph(new Chunk((IDrawInterface)new LineSeparator(0.0f, 100f, BaseColor.BLACK, 0, 1f))));
            cell17.Border = 0;
            cell17.Colspan = 8;
            cell17.PaddingRight = 20f;
            pdfPtable.AddCell(cell17);
            double num = (double)pdfPtable.WriteSelectedRows(-2, -1, pageSize.GetLeft(5f), pageSize.GetTop(1f), writer.DirectContent);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            Paragraph paragraph = new Paragraph(new Phrase("Party/School Rec:", this.footerFont));
            PdfPTable pdfPtable = new PdfPTable(4);
            pdfPtable.TotalWidth = 600f;
            pdfPtable.SetWidths(new int[4] { 10, 5, 60, 30 });
            pdfPtable.TotalWidth = 600f;
            pdfPtable.LockedWidth = true;
            pdfPtable.DefaultCell.FixedHeight = 20f;
            pdfPtable.DefaultCell.Border = 2;
            PdfPCell cell1 = new PdfPCell(new Phrase(string.Format("Page {0} of", (object)writer.PageNumber), this.footerFont));
            cell1.Border = 0;
            cell1.Colspan = 1;
            cell1.HorizontalAlignment = 2;
            pdfPtable.AddCell(cell1);
            PdfPCell cell2 = new PdfPCell(Image.GetInstance(this.total), true);
            cell2.Border = 0;
            cell2.Colspan = 1;
            cell2.HorizontalAlignment = 0;
            cell2.VerticalAlignment = 5;
            pdfPtable.AddCell(cell2);
            PdfPCell cell3 = new PdfPCell((Phrase)paragraph);
            cell3.Border = 0;
            cell3.Colspan = 1;
            cell3.HorizontalAlignment = 1;
            pdfPtable.AddCell(cell3);
            PdfPCell cell4 = new PdfPCell((Phrase)new Paragraph(new Phrase("THANK YOU", this.footerFont)));
            cell4.Border = 0;
            cell4.Colspan = 1;
            cell4.HorizontalAlignment = 0;
            pdfPtable.AddCell(cell4);
            double num = (double)pdfPtable.WriteSelectedRows(0, -1, 10f, 20f, writer.DirectContent);
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            ColumnText.ShowTextAligned((PdfContentByte)this.total, 0, new Phrase(writer.PageNumber.ToString(), this.footerFont), 0.0f, 4f, 0.0f);
        }
    }

}