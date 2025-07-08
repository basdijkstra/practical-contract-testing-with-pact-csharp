using NUnit.Framework;
using System.Net;

namespace CustomerConsumer.Tests
{
    [TestFixture]
    public class AddressClientDeleteTests : PactTestBase
    {
        [Test]
        public async Task DeleteAddress_AddressIdIsValid()
        {
            string addressId = Guid.NewGuid().ToString();

            pact.UponReceiving("Deleting an address ID")
                    .Given("No specific state required")
                    .WithRequest(HttpMethod.Delete, $"/address/{addressId}")
                .WillRespond()
                    .WithStatus(HttpStatusCode.NoContent);

            await pact.VerifyAsync(async ctx => {
                var response = await client.DeleteAddress(addressId);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            });
        }
    }
}
