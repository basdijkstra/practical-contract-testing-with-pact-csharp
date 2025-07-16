
using Newtonsoft.Json;
using NUnit.Framework;
using PactNet.Matchers;
using System.Net;

namespace OrderConsumer.Tests
{
    [TestFixture]
    public class AddressClientGetTests : PactTestBase
    {
        [Test]
        public async Task GetAddress_AddressIdExists()
        {
            string addressId = Guid.NewGuid().ToString();

            var address = new
            {
                Id = Match.Regex(addressId, "[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}"),
                AddressType = Match.Type("billing"),
                Street = Match.Type("Main street"),
                Number = Match.Integer(123),
                City = Match.Type("Los Angeles"),
                ZipCode = Match.Integer(12345),
                State = Match.Type("California"),
                Country = Match.Regex("United States", "United States|Canada")
            };

            pact.UponReceiving("Retrieving an existing address ID")
                    .Given("Address exists", new Dictionary<string, string> { ["id"] = addressId })
                    .WithRequest(HttpMethod.Get, $"/address/{addressId}")
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithJsonBody(address);

            await pact.VerifyAsync(async ctx => {
                var response = await client.GetAddress(addressId);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                string responseBodyAsString = await response.Content.ReadAsStringAsync();
                dynamic responseContent = JsonConvert.DeserializeObject<dynamic>(responseBodyAsString)!;
                Assert.That(responseContent.id.Value, Is.EqualTo(addressId));
            });
        }

        [Test]
        public async Task GetAddress_AddressIdDoesNotExist()
        {
            string addressId = Guid.NewGuid().ToString();

            pact.UponReceiving("Retrieving an address ID that does not exist")
                    .Given("Address does not exist", new Dictionary<string, string> { ["id"] = addressId })
                    .WithRequest(HttpMethod.Get, $"/address/{addressId}")
                .WillRespond()
                    .WithStatus(HttpStatusCode.NotFound);

            await pact.VerifyAsync(async ctx =>
            {
                var response = await client.GetAddress(addressId);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            });
        }

        [Test]
        public async Task GetPOBoxAddress_AddressIdExists()
        {
            string addressId = Guid.NewGuid().ToString();

            var address = new
            {
                Id = Match.Regex(addressId, "[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}"),
                AddressType = Match.Type("billing"),
                PoBox = Match.Type("PO Box 1234"),
                City = Match.Type("Los Angeles"),
                ZipCode = Match.Integer(12345),
                State = Match.Type("California"),
                Country = Match.Regex("United States", "United States|Canada")
            };

            pact.UponReceiving("Retrieving an existing P.O. Box address ID")
                    .Given("PO Box address exists", new Dictionary<string, string> { ["id"] = addressId })
                    .WithRequest(HttpMethod.Get, $"/address/{addressId}")
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithJsonBody(address);

            await pact.VerifyAsync(async ctx => {
                var response = await client.GetAddress(addressId);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                string responseBodyAsString = await response.Content.ReadAsStringAsync();
                dynamic responseContent = JsonConvert.DeserializeObject<dynamic>(responseBodyAsString)!;
                Assert.That(responseContent.id.Value, Is.EqualTo(addressId));
            });
        }
    }
}
