using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDTests
{
    public class PersonsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory) 
        {
            _client = factory.CreateClient();
        }

        #region Index
        [Fact]
        public async Task Index_ToReturnView() 
        {
            HttpResponseMessage response = await _client.GetAsync("/Persons/Index");
            response.Should().BeSuccessful();

            string responseBody = await response.Content.ReadAsStringAsync();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseBody);
            HtmlNode document = htmlDocument.DocumentNode;

            document.QuerySelectorAll("table.persons").Should().NotBeNull();
        }
        #endregion
    }
} 