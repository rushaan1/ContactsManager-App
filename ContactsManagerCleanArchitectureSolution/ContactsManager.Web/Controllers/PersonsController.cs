using ContactsManager_App.Filters;
using ContactsManager_App.Filters.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;

namespace ContactsManager_App.Controllers
{
    [ResponseHeaderFilterFactory("My-Key-From-Global", "My-Value-FromGlobal", 2)]
    public class PersonsController : Controller
    {
        private readonly IPersonAdderService _personAdderService;
        private readonly IPersonGetterService _personGetterService;
        private readonly IPersonSorterService _personSorterService;
        private readonly IPersonUpdaterService _personUpdaterService;
        private readonly IPersonDeleterService _personDeleterService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;
        public PersonsController(ICountriesService countriesService, ILogger<PersonsController> logger, IPersonAdderService personAdderService, IPersonGetterService personGetterService, IPersonSorterService personSorterService, IPersonUpdaterService personUpdaterService, IPersonDeleterService personDeleterService)
        {
            _personAdderService = personAdderService;
            _personGetterService = personGetterService;
            _personSorterService = personSorterService;
            _personUpdaterService = personUpdaterService;
            _personDeleterService = personDeleterService;
            _countriesService = countriesService;
            _logger = logger;
        }


        [Route("/persons/index")]
        [Route("/")]
        [TypeFilter(typeof(PersonsListActionFilter), Order = 4)]
        [ResponseHeaderFilterFactory("My-Key-From-Global", "My-Value-FromGlobal", 2)]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC) 
        {
            _logger.LogInformation("Index Action method of Persons Controller");
            _logger.LogDebug($"searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder}");
            

            List<PersonResponse> persons = await _personGetterService.GetFilteredPersons(searchBy, searchString);
            List<PersonResponse> sortedPersons = await _personSorterService.GetSortedPersons(persons, sortBy, sortOrder);
            

            return View(sortedPersons);
        }

        [HttpGet]
        [Route("/persons/create")]
        public async Task<IActionResult> Create() 
        {
            List<CountryResponse> allCountries = await _countriesService.GetAllCountries();
            ViewBag.Countries = allCountries.Select(c=>new SelectListItem() { Text = c.CountryName, Value = c.CountryId.ToString() });
            return View();
        }

        [HttpPost]
        [Route("/persons/create")]
        [TypeFilter(typeof(PersonsCreateAndEditPostActionFilter))]
        public async Task<IActionResult> Create(PersonAddRequest personRequest) 
        {
            PersonResponse personResponse = await _personAdderService.AddPerson(personRequest);
            Console.WriteLine("Redirecting...");
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("/persons/edit/{personId}")]
        public async Task<IActionResult> Edit(Guid personId) 
        {
            PersonResponse? person = await _personGetterService.GetPersonByPersonId(personId);
            if (person == null)
            {
                return RedirectToAction("Index");
            }

            PersonUpdateRequest updateReq = person.ToPersonUpdateRequest();

            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });

            return View(updateReq);
        }


        [HttpPost]
        [Route("/persons/edit/{personId}")]
        [TypeFilter(typeof(PersonsCreateAndEditPostActionFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest) 
        {
            PersonResponse? personResponse = await _personGetterService.GetPersonByPersonId(personRequest.PersonId);
            if (personResponse == null) 
            {
                return RedirectToAction("Index");
            }

            PersonResponse updated = await _personUpdaterService.UpdatePerson(personRequest);
            return RedirectToAction("Index");
        }


        [HttpGet]
        [Route("/persons/delete/{personId}")]
        public async Task<IActionResult> Delete(Guid personId) 
        {
            PersonResponse? pr = await _personGetterService.GetPersonByPersonId(personId);
            if (pr == null) 
            {
                return RedirectToAction("Index");
            }
            return View(pr);
        }


        [HttpPost]
        [Route("/persons/delete/{personId}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest p) 
        {
            PersonResponse? pr = await _personGetterService.GetPersonByPersonId(p.PersonId);
            if (pr == null) 
            {
                return RedirectToAction("Index");
            }
            await _personDeleterService.DeletePerson(pr.PersonId);
            return RedirectToAction("Index");
        }

        [Route("persons/PersonsPDF")]
        public async Task<IActionResult> PersonsPDF()
        {
            //Get list of persons
            List<PersonResponse> persons = await _personGetterService.GetAllPersons();

            //Return view as pdf
            return new ViewAsPdf("PersonsPDF", persons, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Right = 20, Bottom = 20, Left = 20 },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }

        [Route("persons/PersonsCSV")]
        public async Task<IActionResult> PersonsCSV() 
        {
            MemoryStream stream = await _personGetterService.GetPersonCSV();
            return File(stream, "application/octet-stream", "people.csv");
        }

        [Route("persons/PersonsExcel")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream stream = await _personGetterService.GetPersonsExcel();
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "people.xlsx");
        }
    }
}
