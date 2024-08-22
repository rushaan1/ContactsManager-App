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
        IPersonService _personService;
        ICountriesService _countriesService;
        ILogger<PersonsController> _logger;
        public PersonsController(IPersonService personService, ICountriesService countriesService, ILogger<PersonsController> logger)
        {
            _personService = personService;
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
            

            List<PersonResponse> persons = await _personService.GetFilteredPersons(searchBy, searchString);
            List<PersonResponse> sortedPersons = await _personService.GetSortedPersons(persons, sortBy, sortOrder);
            

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
            PersonResponse personResponse = await _personService.AddPerson(personRequest);
            Console.WriteLine("Redirecting...");
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("/persons/edit/{personId}")]
        public async Task<IActionResult> Edit(Guid personId) 
        {
            PersonResponse? person = await _personService.GetPersonByPersonId(personId);
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
            PersonResponse? personResponse = await _personService.GetPersonByPersonId(personRequest.PersonId);
            if (personResponse == null) 
            {
                return RedirectToAction("Index");
            }

            PersonResponse updated = await _personService.UpdatePerson(personRequest);
            return RedirectToAction("Index");
        }


        [HttpGet]
        [Route("/persons/delete/{personId}")]
        public async Task<IActionResult> Delete(Guid personId) 
        {
            PersonResponse? pr = await _personService.GetPersonByPersonId(personId);
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
            PersonResponse? pr = await _personService.GetPersonByPersonId(p.PersonId);
            if (pr == null) 
            {
                return RedirectToAction("Index");
            }
            await _personService.DeletePerson(pr.PersonId);
            return RedirectToAction("Index");
        }

        [Route("persons/PersonsPDF")]
        public async Task<IActionResult> PersonsPDF()
        {
            //Get list of persons
            List<PersonResponse> persons = await _personService.GetAllPersons();

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
            MemoryStream stream = await _personService.GetPersonCSV();
            return File(stream, "application/octet-stream", "people.csv");
        }

        [Route("persons/PersonsExcel")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream stream = await _personService.GetPersonsExcel();
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "people.xlsx");
        }
    }
}
