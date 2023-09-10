using Claims.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Text;
using Xunit;

namespace Claims.Tests
{
    public class ClaimsControllerTests
    {
        [Fact]
        public async Task Get_Claims()
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(_ =>
                {});

            var client = application.CreateClient();

            var response = await client.GetAsync("/Claims");

            response.EnsureSuccessStatusCode();

            //TODO: Apart from ensuring 200 OK being returned, what else can be asserted?
            var responseContent = await response.Content.ReadAsStringAsync();
            var claims = JsonConvert.DeserializeObject<IEnumerable<Claim>>(responseContent);

            Assert.NotNull(claims);
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }


        [Fact]
        public async Task Create_Claim()
        {
            var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(_ =>
            { });

            var client = application.CreateClient();
            var claim = new Claim()
            {
                Id = "1",
                CoverId = "1",
                DamageCost = 1000,
                Created = DateTime.Today,
                Name = "Name",
                Type = Models.Enums.ClaimTypeEnum.Fire,
            };

            var json = JsonConvert.SerializeObject(claim);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            //var content = new FormUrlEncodedContent(claim);

            var response = await client.PostAsync("/Claims", data);

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

    }
}
