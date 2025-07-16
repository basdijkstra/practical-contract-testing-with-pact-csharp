using NUnit.Framework;
using OrderConsumer.Models;
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace OrderConsumer.Tests
{
    [TestFixture]
    public class PaymentProviderTests
    {
        protected WireMockServer? Server { get; private set; }

        protected static readonly int MOCK_SERVER_PORT = 9876;

        protected static readonly string MOCK_SERVER_BASE_URL = $"http://localhost:{MOCK_SERVER_PORT}";

        [OneTimeSetUp]
        public void StartServer()
        {
            this.Server = WireMockServer.Start(MOCK_SERVER_PORT);
        }

        [Test]
        public async Task GetPayment_ValidExistingOrderId_ShouldYieldHttp200()
        {
            this.SetupStubValidExistingOrderId();

            var response = await new PaymentClient(new Uri(this.Server!.Url!))
                .GetPaymentForOrder("8383a7c3-f831-4f4d-a0a9-015165148af5");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetPayment_ValidNonExistingOrderId_ShouldYieldHttp404()
        {
            this.SetupStubValidNonExistentOrderId();

            var response = await new PaymentClient(new Uri(this.Server!.Url!))
                .GetPaymentForOrder("00000000-0000-0000-0000-000000000000");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        /**
        * TODO: Complete the below test, one that writes a new interaction to the contract, for an HTTP 400 situation.
        *   
        *   First, define the WireMock stub definition by completing the SetupStubInvalidOrderId() method.
        *   
        *   It is very similar to the one for the HTTP 404 interaction, but it should respond to an HTTP GET
        *     to '/payment/invalid-order-id' with an HTTP 400.
        *   
        *   Next, call the GetPaymentForOrder() method on a new PaymentServiceClient instance
        *   (see the other tests) to retrieve the payment details for order 'invalid-order-id'
        *   and verify that it returns a status code HttpStatusCode.BadRequest
        */

        [Test]
        public async Task GetPayment_InvalidOrderId_ShouldYieldHttp400()
        {
            this.SetupStubInvalidOrderId();
        }

        private void SetupStubValidExistingOrderId()
        {
            Payment payment = new Payment()
            {
                Id = Guid.Parse("8383a7c3-f831-4f4d-a0a9-015165148af5"),
                OrderId = Guid.Parse("228aa55c-393c-411b-9410-4a995480e78e"),
                Status = "payment_complete",
                Amount = 42,
                Description = $"Payment for order 228aa55c-393c-411b-9410-4a995480e78e"
            };

            this.Server?
                .WithConsumer("order_consumer")
                .WithProvider("payment_provider")
                .Given(Request.Create().WithPath("/payment/8383a7c3-f831-4f4d-a0a9-015165148af5").UsingGet())
                .WithTitle("A GET request with a valid existing order ID")
                .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBodyAsJson(payment));
        }

        private void SetupStubValidNonExistentOrderId()
        {
            this.Server?
                .WithConsumer("order_consumer")
                .WithProvider("payment_provider")
                .Given(Request.Create().WithPath("/payment/00000000-0000-0000-0000-000000000000").UsingGet())
                .WithTitle("A GET request with a valid nonexistent order ID")
                .RespondWith(Response.Create()
                .WithStatusCode(404));
        }

        private void SetupStubInvalidOrderId()
        {
            /**
             * TODO: set up the stub implemention in this method.
             */
        }

        [OneTimeTearDown]
        protected void StopServer()
        {
            this.Server?.SavePact(Path.Combine("../../../", "pacts"), "order_consumer-payment_provider.json");

            this.Server?.Stop();
        }
    }
}