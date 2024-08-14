using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;

namespace ContactsManager_App.Controllers
{
    public class PersonsController : Controller
    {
        IPersonService _personService;
        ICountriesService _countriesService;
        public PersonsController(IPersonService personService, ICountriesService countriesService) 
        {
            _personService = personService;
            _countriesService = countriesService;
        }


        [Route("/persons/index")]
        [Route("/")]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC) 
        {
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryId), "Country" },
                { nameof(PersonResponse.Address), "Address" }
            };

            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();

            List<PersonResponse> persons = await _personService.GetFilteredPersons(searchBy, searchString);
            List<PersonResponse> sortedPersons = await _personService.GetSortedPersons(persons, sortBy, sortOrder);
            foreach (PersonResponse p in sortedPersons) 
            {
                Console.WriteLine("Sorted id: "+p.PersonId);
            }
            Console.WriteLine("\n");
            foreach (PersonResponse p in persons)
            {
                Console.WriteLine("Filtered id: " + p.PersonId);
            }
            Console.WriteLine("------------------------------\n");

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
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest) 
        {
            if (!ModelState.IsValid) 
            {
                List<CountryResponse> allCountries = await _countriesService.GetAllCountries();
                ViewBag.Countries = allCountries.Select(c => new SelectListItem() { Text = c.CountryName, Value = c.CountryId.ToString() });
                ViewBag.Errors = ModelState.Values.SelectMany(v=>v.Errors).Select(e=>e.ErrorMessage).ToList();
                return View();
            }
            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
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
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest) 
        {
            PersonResponse? personResponse = await _personService.GetPersonByPersonId(personUpdateRequest.PersonId);
            if (personResponse == null) 
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                PersonResponse updated = await _personService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");
            }
            else 
            {
                List<CountryResponse> allCountries = await _countriesService.GetAllCountries();
                ViewBag.Countries = allCountries.Select(c => new SelectListItem() { Text = c.CountryName, Value = c.CountryId.ToString() });
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(personResponse.ToPersonUpdateRequest());
            }
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
