using Entities;
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
    public class PersonGetterService : IPersonGetterService
    {
        IPersonsRepository _personRepository;
        ILogger<PersonGetterService> _logger;
        IDiagnosticContext _diagnosticContext;
        public PersonGetterService(IPersonsRepository personsRepository, ILogger<PersonGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
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
