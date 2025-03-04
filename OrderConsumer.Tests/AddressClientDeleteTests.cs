﻿using NUnit.Framework;
using System.Net;

namespace OrderConsumer.Tests
{
    [TestFixture]
    public class AddressClientDeleteTests : PactTestBase
    {
        [Test]
        public async Task DeleteAddress_AddressIdIsValid()
        {
            string addressId = Guid.NewGuid().ToString();

            pact.UponReceiving("A request to delete an address by ID")
                    .Given("an address with ID {id} exists", new Dictionary<string, string> { ["id"] = addressId })
                    .WithRequest(HttpMethod.Delete, $"/address/{addressId}")
                .WillRespond()
                    .WithStatus(HttpStatusCode.NoContent);

            await pact.VerifyAsync(async ctx => {
                var response = await client.DeleteAddress(addressId);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            });
        }

        [Test]
        public async Task DeleteAddress_AddressIdIsInvalid()
        {
            pact.UponReceiving("A request to delete an address by ID")
                    .Given($"no specific state required")
                    .WithRequest(HttpMethod.Delete, "/address/invalid_address_id")
                .WillRespond()
                    .WithStatus(HttpStatusCode.BadRequest);

            await pact.VerifyAsync(async ctx => {
                var response = await client.DeleteAddress("invalid_address_id");
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            });
        }
    }
}
