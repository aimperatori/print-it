using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Moq;
using Patagames.Pdf.Net;

namespace PrintIt.Core.Tests
{
    public sealed class PdfPrintServiceTests
    {
        //// [Fact]
        public void Print_IntegrationTest()
        {
            // Arrange
            PdfCommon.Initialize();
            var service = new PdfPrintService(Mock.Of<ILogger<PdfPrintService>>());
            using Stream stream = GetEmbeddedResourceStream("Pdfium.dummy.pdf");

            // Act
            service.Print(stream, "Some printer name", numberOfCopies: 2);
        }

        private static Stream GetEmbeddedResourceStream(string name)
        {
            Type type = typeof(PdfPrintServiceTests);
            return type.Assembly.GetManifestResourceStream(type, name);
        }
    }
}
