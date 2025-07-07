using Newtonsoft.Json;
using NUnit.Framework;
using PactNet.Matchers;
using System.Net;

namespace CustomerConsumer.Tests
{
    [TestFixture]
    public class AddressClientGetTests : PactTestBase
    {
        [Test]
        public async Task GetAddress_AddressIdExists()
        {
            /**
             * TODO: Add the following expectations for the provider response to the existing ones:
             * - The response should contain a field 'ZipCode' with an integer value
             * - The response should contain a field 'State' with a string value
             * - The response should contain a field 'Country' with a value that can only be 'United States' or 'Canada'
             */

            string addressId = Guid.NewGuid().ToString();

            var address = new
            {
                Id = Match.Regex(addressId, "[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}"),
                AddressType = Match.Type("billing"),
                Street = Match.Type("Main street"),
                Number = Match.Integer(123),
                City = Match.Type("Los Angeles")
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
            /**
             * TODO: create a pact that defines the following expectations for the address_provider:
             * - specify that a GET to /address/00000000-0000-0000-0000-000000000000 is to be performed
             * - specify that this request should return an HttpStatusCode.NotFound (HTTP 404)
             * - generate a pact segment from these expectations and returning that
             * You should use a provider state with the exact name 'Address does not exist'
             *   using a parameterized provider state just like we saw in the videos, and just like in the interaction defined above.
             * The implementation is very similar to the one above, but does not need the WithJsonBody() part as we don't expect
             * the provider to return a response body in this situation.
             * 
             * Complete the test by invoking the GetAddress() method on the AddressClient,
             * using '00000000-0000-0000-0000-000000000000' as the address ID to retrieve.
             * Verify that the status code returned is equal to HttpStatusCode.NotFound.
             */
        }

        [Test]
        public async Task GetAddress_AddressIdIsInvalid()
        {
            /**
             * TODO: create a pact that defines the following expectations for the address_provider:
             * - specify that a GET to /address/invalid_address_id is to be performed
             * - specify that this request should return an HttpStatusCode.BadRequest (HTTP 400)
             * - generate a pact segment from these expectations and returning that
             * You should use a provider state with the exact name 'No specific state required'.
             *   This provider state does not take any parameters.
             * 
             * Complete the test by invoking the GetAddress() method on the AddressClient,
             * using 'invalid_address_id' as the address ID to retrieve.
             * Verify that the status code returned is equal to HttpStatusCode.BadRequest.
             */
        }
    }
}
