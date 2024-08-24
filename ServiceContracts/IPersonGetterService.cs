using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IPersonGetterService
    {
        Task<List<PersonResponse>> GetAllPersons();
        Task<PersonResponse?> GetPersonByPersonId(Guid? personId);
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);
        Task<MemoryStream> GetPersonCSV();
        Task<MemoryStream> GetPersonsExcel();
    }
}
