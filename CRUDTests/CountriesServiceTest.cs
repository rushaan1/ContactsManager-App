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
        public async Task AddCountry_NullCountryAddRequest() 
        {
            CountryAddRequest? request = null;

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _countriesService.AddCountry(request);
            });
        }

        [Fact]
        public async Task AddCountry_NullCountryName()
        {
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = null
            };

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(request);
            });
        }

        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            CountryAddRequest? request2 = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(request);
                await _countriesService.AddCountry(request2);
            });
        }

        [Fact]
        public async Task AddCountry_Proper()
        {
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "India"
            };

            CountryResponse? response = await _countriesService.AddCountry(request);
            List<CountryResponse> allCountries = await _countriesService.GetAllCountries();
            Assert.True(response.CountryId != Guid.Empty);
            Assert.Contains(response, allCountries);
        }
        #endregion

        #region GetAllCountries
        [Fact]
        public async Task GetAllCountries_Empty() 
        {
            List<CountryResponse> countryResponses = await _countriesService.GetAllCountries();
            Assert.Empty(countryResponses);
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries() 
        {
            List<CountryAddRequest> countryAddRequests = new List<CountryAddRequest>()
            {
                new CountryAddRequest() { CountryName="UK" },
                new CountryAddRequest() { CountryName="USA" }
            };

            List<CountryResponse> countryAddResponses = new List<CountryResponse>();

            foreach (CountryAddRequest request in countryAddRequests)
            {
                countryAddResponses.Add(await _countriesService.AddCountry(request));
            }

            List<CountryResponse> allCountries = await _countriesService.GetAllCountries();

            foreach (CountryResponse country in countryAddResponses) 
            {
                Assert.Contains(country, allCountries);
            }
        }
        #endregion

        #region GetCountryByCountryId

        [Fact]
        public async Task GetCountryByCountryId_NullCountryId() 
        {
            Guid? countryId = null;
            CountryResponse? cr = await _countriesService.GetCountryByCountryId(countryId);
            Assert.Null(cr);
        }

        [Fact]
        public async Task GetCountryByCountryId_ValidCountryId() 
        {
            CountryAddRequest? request = new CountryAddRequest() 
            {
                CountryName = "Brazil"
            };
            CountryResponse addedCountry = await _countriesService.AddCountry(request);
            CountryResponse? countryByGet = await _countriesService.GetCountryByCountryId(addedCountry.CountryId);

            Assert.Equal(addedCountry, countryByGet);
        }

        #endregion
    }
}
