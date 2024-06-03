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

namespace CRUDTests
{
    public class PersonServiceTest
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;

        public PersonServiceTest() 
        {
            _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
            _personService = new PersonService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options), _countriesService);
            
        }

        #region AddPerson

        [Fact]
        public async Task AddPerson_NullPerson() 
        {
            PersonAddRequest? personAddRequest = null;

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _personService.AddPerson(personAddRequest);
            });
        }

        [Fact]
        public async Task AddPerson_NullPersonName()
        {
            PersonAddRequest? personAddRequest = new PersonAddRequest() 
            {
                PersonName = null
            };

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personService.AddPerson(personAddRequest);
            });
        }

        [Fact]
        public async Task AddPerson_ProperPersonDetails() 
        {
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Mark",
                Address = "Uptown, China",
                Email = "mark.new854@gmail.com",
                CountryId = Guid.NewGuid(),
                Gender = ServiceContracts.Enums.GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-05"),
                ReceiveNewsLetters = true
            };
            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
            List<PersonResponse> allPersons = await _personService.GetAllPersons();

            Assert.True(personResponse.PersonId != Guid.Empty);
            Assert.Contains(personResponse, allPersons);
        }

        #endregion

        #region GetPersonByPersonId

        [Fact]
        public async Task GetPersonByPersonId_NullPersonId() 
        {
            Guid? guid = null;
            PersonResponse? personResponse = await _personService.GetPersonByPersonId(guid);
            Assert.Null(personResponse);
        }

        [Fact]
        public async Task GetPersonByPersonId_ProperPersonId() 
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Thailand" };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Sample name",
                Address = "123, Sample Address, DownTown",
                Email = "example@gmail.com",
                CountryId = countryResponse.CountryId,
                Gender = ServiceContracts.Enums.GenderOptions.Female,
                ReceiveNewsLetters = true,
                DateOfBirth = DateTime.Parse("2000-01-01")
            };

            PersonResponse addedPerson = await _personService.AddPerson(personAddRequest);
            PersonResponse? personResponseFromGet = await _personService.GetPersonByPersonId(addedPerson.PersonId);
            Assert.Equal(addedPerson, personResponseFromGet);
        }

        #endregion

        #region GetAllPersons

        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            //Act
            List<PersonResponse> persons_from_get = await _personService.GetAllPersons();

            //Assert
            Assert.Empty(persons_from_get);
        }


        //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "India" };

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryId = country_response_1.CountryId, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

            PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryId = country_response_2.CountryId, DateOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };

            PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.Male, Address = "address of rahman", CountryId = country_response_2.CountryId, DateOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            
            List<PersonResponse> persons_list_from_get = await _personService.GetAllPersons();

            
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, persons_list_from_get);
            }
        }
        #endregion

        #region GetFilteredPersons

        [Fact]
        public async Task GetFilteredPersons_EmptySearchString()
        {
            CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "India" };

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryId = country_response_1.CountryId, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

            PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryId = country_response_2.CountryId, DateOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };

            PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.Male, Address = "address of rahman", CountryId = country_response_2.CountryId, DateOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }


            List<PersonResponse> persons_list_from_get = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");


            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, persons_list_from_get);
            }
        }

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "India" };

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryId = country_response_1.CountryId, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

            PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryId = country_response_2.CountryId, DateOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };

            PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.Male, Address = "address of rahman", CountryId = country_response_2.CountryId, DateOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }


            List<PersonResponse> persons_list_from_get = await _personService.GetFilteredPersons(nameof(Person.PersonName), "ma");


            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                if (person_response_from_add.PersonName != null) 
                {
                    if (person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase)) 
                    {
                        Assert.Contains(person_response_from_add, persons_list_from_get);
                    }
                }
            }
        }

        #endregion

        #region GetSortedPersons

        [Fact]
        public async Task GetSortedPersons()
        {
            CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "India" };

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryId = country_response_1.CountryId, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

            PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryId = country_response_2.CountryId, DateOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };

            PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.Male, Address = "address of rahman", CountryId = country_response_2.CountryId, DateOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            List<PersonResponse> allPersons = await _personService.GetAllPersons();
            List<PersonResponse> persons_list_from_get = await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp=>temp.PersonName).ToList();

            for (int i = 0; i < person_response_list_from_add.Count; i++) 
            {
                Assert.Equal(person_response_list_from_add[i], persons_list_from_get[i]);
            }
        }

        #endregion

        #region UpdatePerson

        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => {
                //Act
                await _personService.UpdatePerson(person_update_request);
            });
        }


        //When we supply invalid person id, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = new PersonUpdateRequest() { PersonId = Guid.NewGuid() };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => {
                //Act
                await _personService.UpdatePerson(person_update_request);
            });
        }

        [Fact]
        public async Task UpdatePerson_PersonNameIsNull() 
        {
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "John", CountryId = country_response_from_add.CountryId, Email = "john@example.com", Address = "address...", Gender = GenderOptions.Male };
            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            PersonUpdateRequest personUpdateRequest = person_response_from_add.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = null;

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personService.UpdatePerson(personUpdateRequest);
            });
        }

        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "John", CountryId = country_response_from_add.CountryId, Address = "Abc road", DateOfBirth = DateTime.Parse("2000-01-01"), Email = "abc@example.com", Gender = GenderOptions.Male, ReceiveNewsLetters = true };

            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = "William";
            person_update_request.Email = "william@example.com";

            PersonResponse personResponseFromUpdate = await _personService.UpdatePerson(person_update_request);
            PersonResponse? personResponseFromGet = await _personService.GetPersonByPersonId(personResponseFromUpdate.PersonId);
            Assert.Equal(personResponseFromGet, personResponseFromUpdate);
        }

        #endregion

        #region DeletePerson

        [Fact]
        public async Task DeletePerson_InvalidPersonId() 
        {
            Guid? guid = Guid.NewGuid();
            bool? isDeleted = await _personService.DeletePerson(guid);
            Assert.False(isDeleted);
        }

        [Fact]
        public async Task DeletePerson_ValidPersonId()
        {
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);
            PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "Jones", Address = "address", CountryId = country_response_from_add.CountryId, DateOfBirth = Convert.ToDateTime("2010-01-01"), Email = "jones@example.com", Gender = GenderOptions.Male, ReceiveNewsLetters = true };
            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            bool isDeleted = await _personService.DeletePerson(person_response_from_add.PersonId);
            Assert.True(isDeleted);
        }

        #endregion
    }
}
