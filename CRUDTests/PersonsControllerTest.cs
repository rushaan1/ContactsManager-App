using AutoFixture;
using ContactsManager_App.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDTests
{
    public class PersonsControllerTest
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        private readonly Mock<IPersonService> _personServiceMock;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Fixture _fixture;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;
        public PersonsControllerTest()
        {
            _personServiceMock = new Mock<IPersonService>();
            _countriesServiceMock = new Mock<ICountriesService>();
            _personService = _personServiceMock.Object;
            _countriesService = _countriesServiceMock.Object;
            _fixture = new Fixture();
            _loggerMock = new Mock<ILogger<PersonsController>>();
        }

        #region Index
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithModel() 
        {
            // Arrange
            List<PersonResponse> personResponses = _fixture.Create<List<PersonResponse>>();
            
            PersonsController personsController = new PersonsController(_personService, _countriesService, _loggerMock.Object);
            
            _personServiceMock.Setup(s=>s.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(personResponses);
            _personServiceMock.Setup(s => s.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
                .ReturnsAsync(personResponses);

            // Act
            IActionResult result = await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(personResponses);
        }
        #endregion

        #region Create

        [Fact]
        public async Task CreateIfModelErrors_ToReturnCreateView() 
        {
            // Arrange
            PersonAddRequest addRequest = _fixture.Create<PersonAddRequest>();
            PersonResponse response = _fixture.Create<PersonResponse>();
            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock.Setup(s=>s.GetAllCountries())
                .ReturnsAsync(countries);
            _personServiceMock.Setup(s=>s.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(response);

            PersonsController pc = new PersonsController(_personService, _countriesService, _loggerMock.Object);

            // Act
            pc.ModelState.AddModelError("PersonName", "Person name can't be blank");
            IActionResult result = await pc.Create(addRequest);

            // Assert
            ViewResult vresult = Assert.IsType<ViewResult>(result);
            vresult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
            vresult.ViewData.Model.Should().Be(addRequest);
        }
        
        [Fact]
        public async Task CreateIfNoModelErrors_ToReturnRedirectToIndex()
        {
            // Arrange
            PersonAddRequest addRequest = _fixture.Create<PersonAddRequest>();
            PersonResponse response = _fixture.Create<PersonResponse>();
            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock.Setup(s => s.GetAllCountries())
                .ReturnsAsync(countries);
            _personServiceMock.Setup(s => s.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(response);

            PersonsController pc = new PersonsController(_personService, _countriesService, _loggerMock.Object);

            // Act
            IActionResult result = await pc.Create(addRequest);

            // Assert
            RedirectToActionResult rr = Assert.IsType<RedirectToActionResult>(result);
            rr.ActionName.Should().Be("Index");
        }
        #endregion
    }
}
