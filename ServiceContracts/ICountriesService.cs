using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Interface for manipulating the country entity with business logic.
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Add country to the database.
        /// </summary>
        /// <param name="countryAddRequest">CountryAddRequest supplied which contains country name</param>
        /// <returns>Returns a CountryResponse which contains Country Name and CountryId</returns>
        CountryResponse AddCountry(CountryAddRequest countryAddRequest);
    }
}
