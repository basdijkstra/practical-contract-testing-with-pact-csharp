using NUnit.Framework;
using PactNet;
using System.Text.Json;

namespace OrderConsumer.Tests
{
    public class PactTestBase
    {
        protected IPactBuilderV3 pact;
        protected AddressClient client;
        protected readonly string addressIdExisting = "93edc1a1-5093-4d30-a9c1-da04765553b7";
        protected readonly string addressIdNonexistent = "3514466e-3e58-48b3-ab35-e553b91aa2b3";
        
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
