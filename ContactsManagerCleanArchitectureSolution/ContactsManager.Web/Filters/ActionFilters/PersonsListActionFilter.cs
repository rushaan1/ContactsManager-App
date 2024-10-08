﻿using ContactsManager_App.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ContactsManager_App.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;
        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger) 
        {
            _logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuted));
            PersonsController personsController = (PersonsController)context.Controller;
            IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];
            if (parameters != null) 
            {
                if (parameters.ContainsKey("searchBy")) 
                {
                    personsController.ViewBag.CurrentSearchBy = Convert.ToString(parameters["searchBy"]);
                }
                if (parameters.ContainsKey("searchString"))
                {
                    personsController.ViewBag.CurrentSearchString = Convert.ToString(parameters["searchString"]);
                }
                if (parameters.ContainsKey("sortBy"))
                {
                    personsController.ViewBag.CurrentSortBy = Convert.ToString(parameters["sortBy"]);
                }
                else 
                {
                    personsController.ViewBag.CurrentSortBy = nameof(PersonResponse.PersonName);
                }
                if (parameters.ContainsKey("sortOrder"))
                {
                    personsController.ViewBag.CurrentSortOrder = Convert.ToString(parameters["sortOrder"]);
                }
                else 
                {
                    personsController.ViewBag.CurrentSortOrder = nameof(SortOrderOptions.ASC);
                }

                personsController.ViewBag.SearchFields = new Dictionary<string, string>()
                    {
                        { nameof(PersonResponse.PersonName), "Person Name" },
                        { nameof(PersonResponse.Email), "Email" },
                        { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                        { nameof(PersonResponse.Gender), "Gender" },
                        { nameof(PersonResponse.CountryId), "Country" },
                        { nameof(PersonResponse.Address), "Address" }
                    };
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items["arguments"] = context.ActionArguments;
            _logger.LogInformation("{FilterName}.{MethodName} method",nameof(PersonsListActionFilter),nameof(OnActionExecuting));

            if (context.ActionArguments.ContainsKey("searchBy")) 
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

                if (!string.IsNullOrEmpty(searchBy)) 
                {
                    List<string> searchByOptions = new List<string>() 
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.CountryId),
                        nameof(PersonResponse.Address)
                    };

                    if (searchByOptions.Any(temp => temp == searchBy) == false) 
                    {
                        _logger.LogInformation("searchBy actual value {searchBy}", searchBy);
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                        _logger.LogInformation("searchBy updated value {searchBy}", searchBy);
                    }
                }
            }
        }
    }
}
