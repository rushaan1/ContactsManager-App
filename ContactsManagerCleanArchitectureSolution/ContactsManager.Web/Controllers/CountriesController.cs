using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace ContactsManager_App.Controllers
{
    public class CountriesController : Controller
    {
        ICountriesService _countriesService;
        public CountriesController(ICountriesService countriesService) 
        {
            _countriesService = countriesService;
        }

        [Route("countries/UploadFromExcel")]
        public IActionResult UploadFromExcel()
        {
            return View();
        }

        [HttpPost]
        [Route("countries/UploadFromExcel")]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelFile) 
        {
            if (excelFile != null && excelFile.Length > 0 && Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase)) 
            {
                int countriesInserted = await _countriesService.UploadCountriesFromExcelFile(excelFile);
                ViewBag.Message = $"{countriesInserted} Countries Inserted.";
                return View();
            }
            ViewBag.ErrorMessage = "Please upload an xlsx file!";
            return View();
        }
    }
}
