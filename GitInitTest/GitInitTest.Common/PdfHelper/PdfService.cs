using SelectPdf;
using System;
using System.Drawing;

namespace GitInitTest.Common.PdfHelper
{
    public interface IPdfFileService
    {
        Byte[] ConvertToPdf(String html);
    }

    public class PdfFileService : IPdfFileService
    {
        public PdfFileService(ISelectPdfConfiguration selectPdfSettings)
        {
            SelectPdf.GlobalProperties.LicenseKey = selectPdfSettings.LicenseKey;
        }

        public Byte[] ConvertToPdf(String html)
        {
            //string htmlString = html;
            //string baseUrl = "";

            string pdf_page_size = "Custom";
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                pdf_page_size, true);

            string pdf_orientation = "Landscape";
            PdfPageOrientation pdfOrientation =
                (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                pdf_orientation, true);

            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = 1200;
            converter.Options.WebPageHeight = 0;

            PdfTextSection txtPageNum = new PdfTextSection(0, 10, "Page: {page_number} of {total_pages}  ", new Font("Times New Roman", 10));
            txtPageNum.HorizontalAlign = PdfTextHorizontalAlign.Right;
            converter.Footer.Add(txtPageNum);
            PdfTextSection txtDate = new PdfTextSection(0, 10, DateTime.Now.ToLongDateString(), new Font("Times New Roman", 10));
            txtDate.HorizontalAlign = PdfTextHorizontalAlign.Left;
            converter.Footer.Add(txtDate);
            converter.Options.DisplayFooter = true;

            PdfDocument doc = converter.ConvertUrl(html);

            // save pdf document
            byte[] pdf = doc.Save();

            // close pdf document
            doc.Close();
            return pdf;
            // return resulted pdf document
        }
    }
}