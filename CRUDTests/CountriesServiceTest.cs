using ServiceContracts;
using Services;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContracts.DTO;
using Microsoft.EntityFrameworkCore;
using Entities;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        public readonly ICountriesService _countriesService;

        public CountriesServiceTest() 
        {
            _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
        }

        #region AddCountry
        [Fact]
        public void AddCountry_NullCountryAddRequest() 
        {
            CountryAddRequest? request = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                _countriesService.AddCountry(request);
            });
        }

        [Fact]
        public void AddCountry_NullCountryName()
        {
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = null
            };

            Assert.Throws<ArgumentException>(() =>
            {
                _countriesService.AddCountry(request);
            });
        }

        [Fact]
        public void AddCountry_DuplicateCountryName()
        {
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryAddRequest? request2 = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            Assert.Throws<ArgumentException>(() =>
            {
                _countriesService.AddCountry(request);
                _countriesService.AddCountry(request2);
            });
        }

        [Fact]
        public void AddCountry_Proper()
        {
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "India"
            };

            CountryResponse? response = _countriesService.AddCountry(request);
            List<CountryResponse> allCountries = _countriesService.GetAllCountries();
            Assert.True(response.CountryId != Guid.Empty);
            Assert.Contains(response, allCountries);
        }
        #endregion

        #region GetAllCountries
        [Fact]
        public void GetAllCountries_Empty() 
        {
            List<CountryResponse> countryResponses = _countriesService.GetAllCountries();
            Assert.Empty(countryResponses);
        }

        [Fact]
        public void GetAllCountries_AddFewCountries() 
        {
            List<CountryAddRequest> countryAddRequests = new List<CountryAddRequest>()
            {
                new CountryAddRequest() { CountryName="UK" },
                new CountryAddRequest() { CountryName="USA" }
            };

            List<CountryResponse> countryAddResponses = new List<CountryResponse>();

            foreach (CountryAddRequest request in countryAddRequests)
            {
                countryAddResponses.Add(_countriesService.AddCountry(request));
            }

            List<CountryResponse> allCountries = _countriesService.GetAllCountries();

            foreach (CountryResponse country in countryAddResponses) 
            {
                Assert.Contains(country, allCountries);
            }
        }
        #endregion

        #region GetCountryByCountryId

        [Fact]
        public void GetCountryByCountryId_NullCountryId() 
        {
            Guid? countryId = null;
            CountryResponse? cr = _countriesService.GetCountryByCountryId(countryId);
            Assert.Null(cr);
        }

        [Fact]
        public void GetCountryByCountryId_ValidCountryId() 
        {
            CountryAddRequest? request = new CountryAddRequest() 
            {
                CountryName = "Brazil"
            };
            CountryResponse addedCountry = _countriesService.AddCountry(request);
            CountryResponse? countryByGet = _countriesService.GetCountryByCountryId(addedCountry.CountryId);

            Assert.Equal(addedCountry, countryByGet);
        }

        #endregion
    }
}
