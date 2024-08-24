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
    public class PersonDeleterService : IPersonDeleterService
    {
        IPersonsRepository _personRepository;
        ILogger<PersonDeleterService> _logger;
        IDiagnosticContext _diagnosticContext;
        public PersonDeleterService(IPersonsRepository personsRepository, ILogger<PersonDeleterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
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
    }
}
