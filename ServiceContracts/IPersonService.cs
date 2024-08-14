﻿using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IPersonService
    {
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);
        Task<List<PersonResponse>> GetAllPersons();
        Task<PersonResponse?> GetPersonByPersonId(Guid? personId);
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
        Task<bool> DeletePerson(Guid? personId);
        Task<MemoryStream> GetPersonCSV();
        Task<MemoryStream> GetPersonsExcel();
    }
}
