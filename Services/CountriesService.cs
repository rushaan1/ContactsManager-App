using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly PersonsDbContext _db;
        public CountriesService(PersonsDbContext personsDbContext) 
        {
            _db = personsDbContext;
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
            if (_db.Countries.Where(temp => temp.CountryName == countryAddRequest.CountryName).Count() > 0) 
            {
                throw new ArgumentException("Country name already exists.");
            }
            Country country = countryAddRequest.ToCountry();
            country.CountryId = Guid.NewGuid();
            _db.Countries.Add(country);
            _db.SaveChanges();
            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _db.Countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryId(Guid? countryId)
        {
            if (countryId == null) 
            {
                return null;
            }

            Country? retrievedCountry = _db.Countries.FirstOrDefault(country => country.CountryId == countryId);
            if (retrievedCountry == null) 
            {
                return null;
            }

            return retrievedCountry.ToCountryResponse();
        }
    }
}
