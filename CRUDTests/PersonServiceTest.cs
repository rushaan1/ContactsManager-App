using ServiceContracts;
using Services;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Entities;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using Moq;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using System.Linq.Expressions;

namespace CRUDTests
{
    public class PersonServiceTest
    {
        private readonly IPersonService _personService;
        private readonly IFixture _fixture;
        private readonly Mock<IPersonsRepository> _personRepositoryMock;
        private readonly IPersonsRepository _personsRepository;

        public PersonServiceTest() 
        {
            _fixture = new Fixture();
            _personRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personRepositoryMock.Object;
            _personService = new PersonService(_personsRepository);
            
        }

        #region AddPerson

        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException() 
        {
            PersonAddRequest? personAddRequest = null;

            Func<Task> action = async () =>
            {
                await _personService.AddPerson(personAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddPerson_NullPersonName_ToBeArgumentException()
        {
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.PersonName, null as string)
                .Create();
            Person person = personAddRequest.ToPerson();
            _personRepositoryMock.Setup(r => r.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            Func<Task> action = async () =>
            {
                await _personService.AddPerson(personAddRequest);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful() 
        {
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "someone@example.com").Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse personResponseExpected = person.ToPersonResponse();
            _personRepositoryMock.Setup(r => r.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
            personResponseExpected.PersonId = personResponse.PersonId;
            
            personResponse.PersonId.Should().NotBe(Guid.Empty);
            personResponse.Should().Be(personResponseExpected);
        }

        #endregion

        #region GetPersonByPersonId

        [Fact]
        public async Task GetPersonByPersonId_NullPersonId() 
        {
            Guid? guid = null;
            PersonResponse? personResponse = await _personService.GetPersonByPersonId(guid);
            personResponse.Should().BeNull();
        }

        [Fact]
        public async Task GetPersonByPersonId_ProperPersonId_ToBeSuccessful() 
        {

            Person person = _fixture.Build<Person>()
                .With(p => p.Email, "someone@example.com")
                .With(p=>p.Country, null as Country)
                .Create();

            PersonResponse expected = person.ToPersonResponse();
            _personRepositoryMock.Setup(r=>r.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);
            PersonResponse? personResponseFromGet = await _personService.GetPersonByPersonId(person.PersonId);
            personResponseFromGet.Should().Be(expected);
        }

        #endregion

        #region GetAllPersons

        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            //Arrange
            List<Person> people = new List<Person>();
            _personRepositoryMock.Setup(r => r.GetAllPersons())
                .ReturnsAsync(people);
            //Act
            List<PersonResponse> persons_from_get = await _personService.GetAllPersons();

            //Assert
            persons_from_get.Should().BeEmpty();
        }


        //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {

            List<Person> persons = new List<Person>() 
            {
                _fixture.Build<Person>()
                .With(r => r.Email, "someone@example.com")
                .With(r=>r.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(r => r.Email, "someone2@example.com")
                .With(r=>r.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(r => r.Email, "someone3@example.com")
                .With(r=>r.Country, null as Country)
                .Create()
            };

            List<PersonResponse> expected = persons.Select(p => p.ToPersonResponse()).ToList();
            _personRepositoryMock.Setup(r => r.GetAllPersons())
                .ReturnsAsync(persons);
            List<PersonResponse> persons_list_from_get = await _personService.GetAllPersons();

            
            persons_list_from_get.Should().BeEquivalentTo(expected);
        }
        #endregion

        #region GetFilteredPersons

        [Fact]
        public async Task GetFilteredPersons_EmptySearchString_ToBeSuccessful()
        {

            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(r => r.Email, "someone@example.com")
                .With(r=>r.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(r => r.Email, "someone2@example.com")
                .With(r=>r.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(r => r.Email, "someone3@example.com")
                .With(r=>r.Country, null as Country)
                .Create()
            };

            List<PersonResponse> expected = persons.Select(p => p.ToPersonResponse()).ToList();
            _personRepositoryMock.Setup(r => r.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);


            List<PersonResponse> persons_list_from_get = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");


            persons_list_from_get.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {

            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(r => r.Email, "someone@example.com")
                .With(r=>r.Country, null as Country)
                .Create(),  

                _fixture.Build<Person>()
                .With(r => r.Email, "someone2@example.com")
                .With(r=>r.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(r => r.Email, "someone3@example.com")
                .With(r=>r.Country, null as Country)
                .Create()
            };

            List<PersonResponse> expected = persons.Select(p => p.ToPersonResponse()).ToList();
            _personRepositoryMock.Setup(r => r.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);


            List<PersonResponse> persons_list_from_get = await _personService.GetFilteredPersons(nameof(Person.PersonName), "sa");


            persons_list_from_get.Should().BeEquivalentTo(expected);
        }
        #endregion

        #region GetSortedPersons

        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(r => r.Email, "someone@example.com")
                .With(r=>r.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(r => r.Email, "someone2@example.com")
                .With(r=>r.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(r => r.Email, "someone3@example.com")
                .With(r=>r.Country, null as Country)
                .Create()
            };

            _personRepositoryMock.Setup(r => r.GetAllPersons())
                .ReturnsAsync(persons);

            List<PersonResponse> allPersons = await _personService.GetAllPersons();
            List<PersonResponse> persons_list_from_get = await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            persons_list_from_get.Should().BeInDescendingOrder(temp=>temp.PersonName);
        }

        #endregion

        #region UpdatePerson

        [Fact]
        public async Task UpdatePerson_NullPersonToBeArgumentNullException()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = null;

            //Assert
            Func<Task> action = async () => {
                //Act
                await _personService.UpdatePerson(person_update_request);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }


        //When we supply invalid person id, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
        {
            //Arrange
            Person? person = _fixture.Build<Person>()
                .With(p => p.Email, "someone@example.com")
                .With(p => p.Country, null as Country)
                .With(p => p.Gender, "Male")
                .Create();

            _personRepositoryMock.Setup(r => r.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(null as Person);

            //Assert
            Func<Task> action = async () => {
                //Act
                await _personService.UpdatePerson(person.ToPersonResponse().ToPersonUpdateRequest());
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException() 
        {
            Person? person = _fixture.Build<Person>()
                .With(p => p.Email, "someone@example.com")
                .With(p => p.Country, null as Country)
                .With(p => p.Gender, "Male")
                .With(p=> p.PersonName, null as string)
                .Create();

            PersonUpdateRequest personUpdateRequest = person.ToPersonResponse().ToPersonUpdateRequest();

            Func<Task> action = async () =>
            {
                await _personService.UpdatePerson(personUpdateRequest);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation_ToBeSuccessful()
        {
            Person? person = _fixture.Build<Person>()
                .With(p => p.Email, "someone@example.com")
                .With(p=>p.Country, null as Country)
                .With(p=>p.Gender, "Male")
                .Create();

            _personRepositoryMock.Setup(r => r.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            _personRepositoryMock.Setup(r=>r.UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            PersonResponse expected = person.ToPersonResponse();
            PersonResponse personResponseFromUpdate = await _personService.UpdatePerson(expected.ToPersonUpdateRequest());
            personResponseFromUpdate.Should().Be(expected);
        }

        #endregion

        #region DeletePerson

        [Fact]
        public async Task DeletePerson_InvalidPersonId() 
        {
            Guid? guid = Guid.NewGuid();
            _personRepositoryMock.Setup(r => r.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(null as Person);
            bool? isDeleted = await _personService.DeletePerson(guid);
            isDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task DeletePerson_ValidPersonId()
        {
            Person? person = _fixture.Build<Person>()
                .With(p => p.Email, "someone@example.com")
                .With(p => p.Country, null as Country)
                .With(p => p.Gender, "Male")
                .Create();
            _personRepositoryMock.Setup(r => r.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            _personRepositoryMock.Setup(r => r.DeletePersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            bool isDeleted = await _personService.DeletePerson(person.PersonId);
            isDeleted.Should().BeTrue();
        }

        #endregion
    }
}
