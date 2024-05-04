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
        /// <summary>
        /// Get list of all countries.
        /// </summary>
        /// <returns>Returns a list of CountryResponse</returns>
        List<CountryResponse> GetAllCountries();
        /// <summary>
        /// Get the corresponding country by providing the country id.
        /// </summary>
        /// <param name="countryId">CountryId which is a Guid needs to be supplied.</param>
        /// <returns>Returns the corresponding country.</returns>
        CountryResponse? GetCountryByCountryId(Guid? countryId);
    }
}
