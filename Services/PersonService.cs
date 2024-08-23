using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using OfficeOpenXml;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;
using Exceptions;

namespace Services
{
    public class PersonService : IPersonService
    {
        IPersonsRepository _personRepository;
        ILogger<PersonService> _logger;
        IDiagnosticContext _diagnosticContext;
        public PersonService(IPersonsRepository personsRepository, ILogger<PersonService> logger, IDiagnosticContext diagnosticContext)
        {
            _personRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }
            if (string.IsNullOrEmpty(personAddRequest.PersonName))
            {
                throw new ArgumentException("Person name can't be null or empty");
            }

            ValidationHelper.ModelValidation(personAddRequest);

            Person person = personAddRequest.ToPerson();
            person.PersonId = Guid.NewGuid();
            await _personRepository.AddPerson(person);
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            var persons = await _personRepository.GetAllPersons();

            return persons
              .Select(temp => temp.ToPersonResponse()).ToList();
        }

        public async Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
        {
            if (personId == null)
            {
                return null;
            }
            Person? person = await _personRepository.GetPersonByPersonID(personId.Value);
            if (person == null)
            {
                return null;
            }

            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            using (Operation.Time("Time for fetching filtered person"))
            {
                _logger.LogInformation("GetFilteredPersons of PersonService");

                if (String.IsNullOrEmpty(searchString) || String.IsNullOrWhiteSpace(searchString))
                {
                    //List<Person> everyone = await _personRepository.GetAllPersons();
                    //return everyone.Select(p => p.ToPersonResponse()).ToList();
                    List<Person> ppl = await _personRepository.GetFilteredPersons(p => true);
                    _diagnosticContext.Set("Persons", ppl);
                    return ppl.Select(p => p.ToPersonResponse()).ToList();
                }
                List<Person> matchingPersons = searchBy switch
                {
                    nameof(PersonResponse.PersonName) =>
                        await _personRepository.GetFilteredPersons(p => p.PersonName.Contains(searchString)),


                    nameof(PersonResponse.Email) =>
                        await _personRepository.GetFilteredPersons(p => p.Email.Contains(searchString)),


                    nameof(PersonResponse.DateOfBirth) =>
                        await _personRepository.GetFilteredPersons(p => p.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString)),


                    nameof(PersonResponse.Gender) =>
                        await _personRepository.GetFilteredPersons(p => p.Gender.Contains(searchString)),


                    nameof(PersonResponse.CountryId) =>
                        await _personRepository.GetFilteredPersons(p => p.Country.CountryName.Contains(searchString)),


                    nameof(PersonResponse.Address) =>
                        await _personRepository.GetFilteredPersons(p => p.Address.Contains(searchString)),


                    _ => await _personRepository.GetAllPersons()

                };

                return matchingPersons.Select(p => p.ToPersonResponse()).ToList();
            }
        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            _logger.LogInformation("GetSortedPersons of PersonService");

            if (string.IsNullOrEmpty(sortBy)) 
            {
                return allPersons;
            }

            List<PersonResponse> sortedPersons = (sortBy, sortOrder)
            switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC)
                => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC)
                => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC)
                => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC)
                => allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC)
                => allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Country), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC)
                => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC)
                => allPersons.OrderBy(temp => temp.Age).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(temp => temp.Age).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC)
                => allPersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),
                
                _ => allPersons
            };

            return sortedPersons;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null) 
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));
            }

            ValidationHelper.ModelValidation(personUpdateRequest);

            Person? matchingPerson = await _personRepository.GetPersonByPersonID(personUpdateRequest.PersonId);

            if (matchingPerson == null) 
            {
                throw new InvalidPersonIdException("Given person id doesn't exist.");
            }

            
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryId = personUpdateRequest.CountryId;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
            await _personRepository.UpdatePerson(matchingPerson);
            return matchingPerson.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? personId)
        {
            if (personId == null) 
            {
                throw new ArgumentNullException(nameof(personId));
            }

            Person? person = await _personRepository.GetPersonByPersonID(personId.Value);
            if (person == null) 
            {
                return false;
            }
            await _personRepository.DeletePersonByPersonID(personId.Value);
            return true;
        }

        public async Task<MemoryStream> GetPersonCSV() 
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);
            CsvWriter writer = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture));

            writer.WriteField(nameof(PersonResponse.PersonName));
            writer.WriteField(nameof(PersonResponse.Email));
            writer.WriteField(nameof(PersonResponse.DateOfBirth));
            writer.WriteField(nameof(PersonResponse.Age));
            writer.WriteField(nameof(PersonResponse.Gender));
            writer.WriteField(nameof(PersonResponse.Country));
            writer.WriteField(nameof(PersonResponse.Address));
            writer.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
            writer.NextRecord();

            List<PersonResponse> ppl = await GetAllPersons();

            foreach (PersonResponse person in ppl) 
            {
                writer.WriteField(person.PersonName);
                writer.WriteField(person.Email);
                writer.WriteField(person.DateOfBirth.HasValue ? person.DateOfBirth.Value.ToString("yyyy-MM-dd") : "");
                writer.WriteField(person.Age);
                writer.WriteField(person.Gender);
                writer.WriteField(person.Country);
                writer.WriteField(person.Address);
                writer.WriteField(person.ReceiveNewsLetters);
                writer.NextRecord();
                writer.Flush();
            }

            memoryStream.Position = 0;

            return memoryStream; 
        }
        public async Task<MemoryStream> GetPersonsExcel() 
        {
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage ep = new ExcelPackage(memoryStream)) 
            {
                ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("PeopleSheet");
                sheet.Cells["A1"].Value = nameof(PersonResponse.PersonName);
                sheet.Cells["B1"].Value = nameof(PersonResponse.Email);
                sheet.Cells["C1"].Value = nameof(PersonResponse.DateOfBirth);
                sheet.Cells["D1"].Value = nameof(PersonResponse.Age);
                sheet.Cells["E1"].Value = nameof(PersonResponse.Gender);
                sheet.Cells["F1"].Value = nameof(PersonResponse.Country);
                sheet.Cells["G1"].Value = nameof(PersonResponse.Address);
                sheet.Cells["H1"].Value = nameof(PersonResponse.ReceiveNewsLetters);

                using (ExcelRange headerCells = sheet.Cells["A1:H1"]) 
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headerCells.Style.Font.Bold = true;
                }

                int row = 2;
                var ppl = await GetAllPersons();
                foreach (var p in ppl) 
                {
                    sheet.Cells[row, 1].Value = p.PersonName;
                    sheet.Cells[row, 2].Value = p.Email;
                    if (p.DateOfBirth.HasValue)
                        sheet.Cells[row, 3].Value = p.DateOfBirth.Value.ToString("yyyy-MM-dd");
                    sheet.Cells[row, 4].Value = p.Age;
                    sheet.Cells[row, 5].Value = p.Gender;
                    sheet.Cells[row, 6].Value = p.Country;
                    sheet.Cells[row, 7].Value = p.Address;
                    sheet.Cells[row, 8].Value = p.ReceiveNewsLetters;

                    row++;
                }
                sheet.Cells[$"A1:H{row}"].AutoFitColumns();
                await ep.SaveAsync();
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
