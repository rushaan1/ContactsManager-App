using ContactsManager_App.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;

namespace ContactsManager_App.Filters.ActionFilters
{
    public class PersonsCreateAndEditPostActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesService _countriesService;
        public PersonsCreateAndEditPostActionFilter(ICountriesService countriesService) 
        {
            _countriesService = countriesService;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is PersonsController personsController)
            {
                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> allCountries = await _countriesService.GetAllCountries();
                    personsController.ViewBag.Countries = allCountries.Select(c => new SelectListItem() { Text = c.CountryName, Value = c.CountryId.ToString() });
                    personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    context.Result = personsController.View(context.ActionArguments["personRequest"]);
                }
                else
                {
                    await next();
                }
            }
            else 
            {
                await next();
            }
        }
    }
}
