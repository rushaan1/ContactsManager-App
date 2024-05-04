using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        List<Country> _countries;
        public CountriesService() 
        {
            _countries = new List<Country>();
        }
        public CountryResponse AddCountry(CountryAddRequest countryAddRequest)
        {
            if (countryAddRequest == null) 
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest));
            }
            if (_countries.Where(temp => temp.CountryName == countryAddRequest.CountryName).Count() > 0) 
            {
                throw new ArgumentException("Country name already exists.");
            }
            Country country = countryAddRequest.ToCountry();
            country.CountryId = Guid.NewGuid();
            _countries.Add(country);
            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryId(Guid? countryId)
        {
            if (countryId == null) 
            {
                return null;
            }

            Country? retrievedCountry = _countries.FirstOrDefault(country => country.CountryId == countryId);
            if (retrievedCountry == null) 
            {
                return null;
            }

            return retrievedCountry.ToCountryResponse();
        }
    }
}
