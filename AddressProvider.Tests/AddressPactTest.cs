using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PactNet.Verifier;

namespace AddressProvider.Tests
{
    [TestFixture]
    public class AddressPactTest
    {
        private Uri PactServiceUri = new Uri("http://127.0.0.1:9876");

        private IHost? server;
        private PactVerifier? verifier;

        [Test]
        public void VerifyThatAddressServiceHonoursPacts()
        {
            this.server = Host.CreateDefaultBuilder()
                              .ConfigureWebHostDefaults(webBuilder =>
                              {
                                  webBuilder.UseUrls(PactServiceUri.ToString());
                                  webBuilder.UseStartup<TestStartup>();
                              })
                              .Build();

            this.server.Start();

            this.verifier = new PactVerifier("address_provider", new PactVerifierConfig()
            {
                LogLevel = PactNet.PactLogLevel.Information
            });

            var pactFolder = new DirectoryInfo(Path.Join("..", "..", "..", "pacts"));

            //this.verifier!
            //    .WithHttpEndpoint(PactServiceUri)
            //    .WithDirectorySource(pactFolder)
            //    .WithProviderStateUrl(new Uri($"{PactServiceUri}provider-states"))
            //    .Verify();

            this.verifier!
                .WithHttpEndpoint(PactServiceUri)
                .WithPactBrokerSource(new Uri(Environment.GetEnvironmentVariable("PACT_BROKER_BASE_URL")!), options =>
                {
                    options.TokenAuthentication(Environment.GetEnvironmentVariable("PACT_BROKER_TOKEN")).PublishResults("1.0.0");
                })
                .WithProviderStateUrl(new Uri($"{PactServiceUri}provider-states"))
                .Verify();
        }

        [TearDown]
        public void TearDown()
        {
            this.server!.Dispose();
        }
    }
}
