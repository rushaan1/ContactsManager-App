using ServiceContracts;
using Services;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContracts.DTO;
using Entities;
using AutoFixture;
using Moq;
using RepositoryContracts;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        public readonly ICountriesService _countriesService;
        private readonly IFixture _fixture;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;

        public CountriesServiceTest()
        {
            _fixture = new Fixture();
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;
            _countriesService = new CountriesService(_countriesRepository);
        }

        #region AddCountry
        [Fact]
        public async Task AddCountry_NullCountryAddRequest_ToBeArgumentNullException() 
        {
            CountryAddRequest? request = null;

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _countriesService.AddCountry(request);
            });
        }

        [Fact]
        public async Task AddCountry_NullCountryName_ToBeArgumentException()
        {
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
                .With(c=>c.CountryName, null as string)
                .Create(); 


            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(request);
            });
        }

        [Fact]
        public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
        {
            CountryAddRequest? request1 = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, "India")
                .Create();
            CountryAddRequest? request2 = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, "India")
                .Create();

            _countriesRepositoryMock.Setup(r => r.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(request1.ToCountry());

            _countriesRepositoryMock.Setup(r => r.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(null as Country);

            CountryResponse added1 = await _countriesService.AddCountry(request1);

            _countriesRepositoryMock.Setup(r => r.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(request1.ToCountry());

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                
                await _countriesService.AddCountry(request2);
            });
        }

        [Fact]
        public async Task AddCountry_Proper_ToBeSuccessful()
        {
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, "India")
                .Create();

            Country country = request.ToCountry();  
            CountryResponse countryResponse = country.ToCountryResponse();

            _countriesRepositoryMock.Setup(r => r.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);

            _countriesRepositoryMock.Setup(r => r.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(null as Country);

            CountryResponse? response = await _countriesService.AddCountry(request);
            country.CountryId = response.CountryId;
            countryResponse.CountryId = response.CountryId;

            Assert.True(response.CountryId != Guid.Empty);
            Assert.Equal(response, countryResponse);
        }
        #endregion

        #region GetAllCountries
        [Fact]
        public async Task GetAllCountries_Empty() 
        {
            _countriesRepositoryMock.Setup(r => r.GetAllCountries())
                .ReturnsAsync(new List<Country>());
            List<CountryResponse> countryResponses = await _countriesService.GetAllCountries();
            Assert.Empty(countryResponses);
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries() 
        {
            List<Country> countries = new List<Country>()
            {
                _fixture.Build<Country>().With(c=>c.Persons, null as List<Person>).Create(),
                _fixture.Build<Country>().With(c=>c.Persons, null as List<Person>).Create()
            };
            List<CountryResponse> expected = countries.Select(c => c.ToCountryResponse()).ToList();
           
            _countriesRepositoryMock.Setup(r => r.GetAllCountries())
                .ReturnsAsync(countries);
            
            List<CountryResponse> allCountries = await _countriesService.GetAllCountries();
            Assert.Equal(expected, allCountries);
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
            Country country = _fixture.Build<Country>().With(c => c.Persons, null as List<Person>).Create();
            CountryResponse expected = country.ToCountryResponse();
            _countriesRepositoryMock.Setup(r => r.GetCountryByCountryID(It.IsAny<Guid>()))
                .ReturnsAsync(country);
            CountryResponse? countryByGet = await _countriesService.GetCountryByCountryId(country.CountryId);

            Assert.Equal(countryByGet, expected);
        }

        #endregion
    }
}
