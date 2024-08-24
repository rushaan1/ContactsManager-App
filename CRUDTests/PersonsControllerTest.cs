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
        private readonly IPersonGetterService _personGetterService;
        private readonly IPersonAdderService _personAdderService;
        private readonly IPersonSorterService _personSorterService;
        private readonly IPersonDeleterService _personDeleterService;
        private readonly IPersonUpdaterService _personUpdaterService;

        private readonly ICountriesService _countriesService;

        private readonly Mock<IPersonAdderService> _personAdderServiceMock;
        private readonly Mock<IPersonGetterService> _personGetterServiceMock;
        private readonly Mock<IPersonSorterService> _personSorterServiceMock;
        private readonly Mock<IPersonDeleterService> _personDeleterServiceMock;
        private readonly Mock<IPersonUpdaterService> _personUpdaterServiceMock;

        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Fixture _fixture;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;
        public PersonsControllerTest()
        {
            _personAdderServiceMock = new Mock<IPersonAdderService>();
            _personGetterServiceMock = new Mock<IPersonGetterService>();
            _personSorterServiceMock = new Mock<IPersonSorterService>();
            _personDeleterServiceMock = new Mock<IPersonDeleterService>();
            _personUpdaterServiceMock = new Mock<IPersonUpdaterService>();

            _countriesServiceMock = new Mock<ICountriesService>();

            _personGetterService = _personGetterServiceMock.Object;
            _personAdderService = _personAdderServiceMock.Object;
            _personSorterService = _personSorterServiceMock.Object;
            _personDeleterService = _personDeleterServiceMock.Object;
            _personUpdaterService = _personUpdaterServiceMock.Object;

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
            
            PersonsController personsController = new PersonsController(_countriesService, _loggerMock.Object, _personAdderService, _personGetterService, _personSorterService, _personUpdaterService, _personDeleterService);

            _personGetterServiceMock.Setup(s=>s.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(personResponses);
            _personSorterServiceMock.Setup(s => s.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
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
        public async Task CreateIfNoModelErrors_ToReturnRedirectToIndex()
        {
            // Arrange
            PersonAddRequest addRequest = _fixture.Create<PersonAddRequest>();
            PersonResponse response = _fixture.Create<PersonResponse>();
            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock.Setup(s => s.GetAllCountries())
                .ReturnsAsync(countries);
            _personAdderServiceMock.Setup(s => s.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(response);

            PersonsController pc = new PersonsController(_countriesService, _loggerMock.Object, _personAdderService, _personGetterService, _personSorterService, _personUpdaterService, _personDeleterService);
            // Act
            IActionResult result = await pc.Create(addRequest);

            // Assert
            RedirectToActionResult rr = Assert.IsType<RedirectToActionResult>(result);
            rr.ActionName.Should().Be("Index");
        }
        #endregion
    }
}
