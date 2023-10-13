using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using LibrarySystem.Data;

namespace LibrarySystem.Controllers
{
    public partial class ExportLibraryController : ExportController
    {
        private readonly LibraryContext context;
        private readonly LibraryService service;

        public ExportLibraryController(LibraryContext context, LibraryService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/Library/books/csv")]
        [HttpGet("/export/Library/books/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportBooksToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetBooks(), Request.Query), fileName);
        }

        [HttpGet("/export/Library/books/excel")]
        [HttpGet("/export/Library/books/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportBooksToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetBooks(), Request.Query), fileName);
        }

        [HttpGet("/export/Library/categories/csv")]
        [HttpGet("/export/Library/categories/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCategoriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCategories(), Request.Query), fileName);
        }

        [HttpGet("/export/Library/categories/excel")]
        [HttpGet("/export/Library/categories/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCategoriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCategories(), Request.Query), fileName);
        }

        [HttpGet("/export/Library/renthistories/csv")]
        [HttpGet("/export/Library/renthistories/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRentHistoriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetRentHistories(), Request.Query), fileName);
        }

        [HttpGet("/export/Library/renthistories/excel")]
        [HttpGet("/export/Library/renthistories/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportRentHistoriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetRentHistories(), Request.Query), fileName);
        }

        [HttpGet("/export/Library/signins/csv")]
        [HttpGet("/export/Library/signins/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSigninsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetSignins(), Request.Query), fileName);
        }

        [HttpGet("/export/Library/signins/excel")]
        [HttpGet("/export/Library/signins/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSigninsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetSignins(), Request.Query), fileName);
        }

        [HttpGet("/export/Library/users/csv")]
        [HttpGet("/export/Library/users/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUsersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetUsers(), Request.Query), fileName);
        }

        [HttpGet("/export/Library/users/excel")]
        [HttpGet("/export/Library/users/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUsersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetUsers(), Request.Query), fileName);
        }
    }
}
