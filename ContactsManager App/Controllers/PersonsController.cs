using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

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
        public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC) 
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

            List<PersonResponse> persons = _personService.GetFilteredPersons(searchBy, searchString);
            List<PersonResponse> sortedPersons = _personService.GetSortedPersons(persons, sortBy, sortOrder);
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
        public IActionResult Create() 
        {
            List<CountryResponse> allCountries = _countriesService.GetAllCountries();
            ViewBag.Countries = allCountries.Select(c=>new SelectListItem() { Text = c.CountryName, Value = c.CountryId.ToString() });
            return View();
        }

        [HttpPost]
        [Route("/persons/create")]
        public IActionResult Create(PersonAddRequest personAddRequest) 
        {
            if (!ModelState.IsValid) 
            {
                List<CountryResponse> allCountries = _countriesService.GetAllCountries();
                ViewBag.Countries = allCountries.Select(c => new SelectListItem() { Text = c.CountryName, Value = c.CountryId.ToString() });
                ViewBag.Errors = ModelState.Values.SelectMany(v=>v.Errors).Select(e=>e.ErrorMessage).ToList();
                return View();
            }
            PersonResponse personResponse = _personService.AddPerson(personAddRequest);
            Console.WriteLine("Redirecting...");
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("/persons/edit/{personId}")]
        public IActionResult Edit(Guid personId) 
        {
            PersonResponse? person = _personService.GetPersonByPersonId(personId);
            if (person == null)
            {
                return RedirectToAction("Index");
            }

            PersonUpdateRequest updateReq = person.ToPersonUpdateRequest();

            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });

            return View(updateReq);
        }


        [HttpPost]
        [Route("/persons/edit/{personId}")]
        public IActionResult Edit(PersonUpdateRequest personUpdateRequest) 
        {
            PersonResponse? personResponse = _personService.GetPersonByPersonId(personUpdateRequest.PersonId);
            if (personResponse == null) 
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                PersonResponse updated = _personService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");
            }
            else 
            {
                List<CountryResponse> allCountries = _countriesService.GetAllCountries();
                ViewBag.Countries = allCountries.Select(c => new SelectListItem() { Text = c.CountryName, Value = c.CountryId.ToString() });
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(personResponse.ToPersonUpdateRequest());
            }
        }


        [HttpGet]
        [Route("/persons/delete/{personId}")]
        public IActionResult Delete(Guid personId) 
        {
            PersonResponse? pr = _personService.GetPersonByPersonId(personId);
            if (pr == null) 
            {
                return RedirectToAction("Index");
            }
            return View(pr);
        }


        [HttpPost]
        [Route("/persons/delete/{personId}")]
        public IActionResult Delete(PersonUpdateRequest p) 
        {
            PersonResponse? pr = _personService.GetPersonByPersonId(p.PersonId);
            if (pr == null) 
            {
                return RedirectToAction("Index");
            }
            _personService.DeletePerson(pr.PersonId);
            return RedirectToAction("Index");
        }
    }
}
