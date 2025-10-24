using OrderProvider.Orders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PactNet.Verifier;
using System.Text.Json;

namespace OrderProvider.Tests
{
    [TestFixture]
    public class FulfillmentPactTests
    {
        private Uri PactServiceUri = new Uri("http://127.0.0.1:9876");

        private IHost? server;
        private PactVerifier? verifier;
        private JsonSerializerOptions? options;

        [Test]
        public void VerifyThatOrderServiceHonoursPacts()
        {
            this.server = Host.CreateDefaultBuilder()
                              .ConfigureWebHostDefaults(webBuilder =>
                              {
                                  webBuilder.UseUrls(PactServiceUri.ToString());
                                  webBuilder.UseStartup<TestStartup>();
                              })
                              .Build();

            this.server.Start();

            this.options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            this.verifier = new PactVerifier("order_provider", new PactVerifierConfig()
            {
                LogLevel = PactNet.PactLogLevel.Debug,
            });

            this.verifier!
                .WithHttpEndpoint(new Uri("http://localhost"))
                .WithMessages(scenarios =>
                {
                    scenarios.Add("an event indicating that an order has been created", builder =>
                    {
                        builder.WithMetadata(new
                        {
                            ContentType = "application/json",
                        })
                        .WithContent(() => new OrderCreatedEvent(1));
                    });
                }, this.options)
                .WithPactBrokerSource(new Uri(Environment.GetEnvironmentVariable("PACT_BROKER_BASE_URL")!), options =>
                {
                    options
                    .TokenAuthentication(Environment.GetEnvironmentVariable("PACT_BROKER_TOKEN"))
                    .EnablePending()
                    .PublishResults(Environment.GetEnvironmentVariable("ADDRESS_PROVIDER_VERSION") ?? "1.1.0", publishOptions =>
                    {
                        publishOptions.ProviderBranch("main");
                    });
                })
                .WithProviderStateUrl(new Uri($"{PactServiceUri}provider-states"))
                .Verify();
        }

        [TearDown]
        public void TearDown()
        {
            this.server!.Dispose();
            this.verifier?.Dispose();
        }
    }
}
