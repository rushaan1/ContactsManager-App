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
    public class PersonAdderService : IPersonAdderService
    {
        IPersonsRepository _personRepository;
        ILogger<PersonAdderService> _logger;
        IDiagnosticContext _diagnosticContext;
        public PersonAdderService(IPersonsRepository personsRepository, ILogger<PersonAdderService> logger, IDiagnosticContext diagnosticContext)
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
    }
}
