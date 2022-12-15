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
    [Route("print")]
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
                await using Stream pdfStream = request.PdfFile.OpenReadStream();

                if (_printerService.GetInstalledPrinters().Contains(request.PrinterPath) == false)
                {
                    _printerService.InstallPrinter(request.PrinterPath);
                }

                _pdfPrintService.Print(pdfStream,
                    printerName: request.PrinterPath,
                    pageRange: request.PageRange,
                    numberOfCopies: request.Copies ?? 1,
                    request.PdfFile.FileName);

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
        public IFormFile PdfFile { get; set; }

        [Required]
        public string PrinterPath { get; set; }

        public string PageRange { get; set; }

        public int? Copies { get; set; }
    }
}
