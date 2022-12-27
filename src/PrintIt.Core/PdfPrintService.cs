using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Extensions.Logging;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Controls.Wpf;

namespace PrintIt.Core
{
    [ExcludeFromCodeCoverage]
    internal sealed class PdfPrintService : IPdfPrintService
    {
        private readonly ILogger<PdfPrintService> _logger;

        public PdfPrintService(ILogger<PdfPrintService> logger)
        {
            _logger = logger;
        }

        public Stream Merge(Stream pdfStream1, Stream pdfStrea2)
        {
            Stream mergedPdf = new MemoryStream();

            _logger.LogInformation($"Merging PDFs");

            using (var mainDoc = PdfDocument.Load(pdfStream1))
            {
                using (var doc = PdfDocument.Load(pdfStrea2))
                {
                    // Import all pages from document
                    mainDoc.Pages.ImportPages(
                        doc,
                        string.Format("1-{0}", doc.Pages.Count),
                        mainDoc.Pages.Count);
                }

                mainDoc.Save(mergedPdf, SaveFlags.NoIncremental | SaveFlags.ObjectStream);
            }

            return mergedPdf;
        }

        public void Print(Stream pdfStream, string printerName, short numberOfCopies = 1, string documentName = "document")
        {
            var document = PdfDocument.Load(pdfStream);

            _logger.LogInformation($"Printing PDF containing {document.Pages.Count} page(s) to printer '{printerName}'");

            var printDoc = new PdfPrintDocument(document);
            printDoc.PrinterSettings.PrinterName = printerName;
            printDoc.PrinterSettings.Copies = numberOfCopies;
            printDoc.DocumentName = documentName;
            printDoc.Print();
        }
    }

    public interface IPdfPrintService
    {
        Stream Merge(Stream pdfStream1, Stream pdfStrea2);

        void Print(Stream pdfStream, string printerName, short numberOfCopies = 1, string documentName = "document");
    }
}
