using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrintIt.Core;

namespace PrintIt.ServiceHost.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PrintController : ControllerBase
    {
        private readonly IPdfPrintService _pdfPrintService;
        private readonly IPrinterService  _printerService;

        public PrintController(IPdfPrintService pdfPrintService, IPrinterService printerService)
        {
            _pdfPrintService = pdfPrintService;
            _printerService = printerService;
        }

        [HttpPost]
        [Route("from-pdf")]
        public async Task<IActionResult> PrintFromPdf([FromForm] PrintFromTemplateRequest request)
        {
            try
            {
                Stream printStream;

                await using Stream pdfStream1 = request.PdfFile1.OpenReadStream();

                await using Stream pdfStream2 = request.PdfFile2?.OpenReadStream();

                printStream = pdfStream1 != null && pdfStream2 != null ? _pdfPrintService.Merge(pdfStream1, pdfStream2) : pdfStream1;

                if (_printerService.GetInstalledPrinters().Contains(request.PrinterPath) == false)
                {
                    _printerService.InstallPrinter(request.PrinterPath);
                }
                
                _pdfPrintService.Print(printStream,
                    printerName: request.PrinterPath,
                    numberOfCopies: request.Copies ?? 1);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public sealed class PrintFromTemplateRequest
    {
        [Required]
        public IFormFile PdfFile1 { get; set; }
        public IFormFile PdfFile2 { get; set; }
        [Required]
        public string PrinterPath { get; set; }
        [Range(1, 100)]
        public short? Copies { get; set; }
    }
}
