using NUnit.Framework;
using PactNet;
using System.Text.Json;

namespace OrderConsumer.Tests
{
    public class PactTestBase
    {
        protected IPactBuilderV3 pact;
        protected AddressClient client;
        
        private string pactDir = Path.Join("..", "..", "..", "pacts");
        private readonly int port = 9876;

        [SetUp]
        public void SetUp()
        {
            var Config = new PactConfig
            {
                PactDir = pactDir,
                DefaultJsonSettings = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }
            };

            pact = Pact.V3("order_consumer", "address_provider", Config).WithHttpInteractions(port);
            client = new AddressClient(new Uri($"http://localhost:{port}"));
        }
    }
}
