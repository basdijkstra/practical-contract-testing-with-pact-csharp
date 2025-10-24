using Moq;
using NUnit.Framework;
using PactNet;
using System.Text.Json;
using Match = PactNet.Matchers.Match;

namespace FulfillmentConsumer.Tests
{
    [TestFixture]
    public class OrderCreatedTests
    {
        private OrderCreatedConsumer consumer;
        private Mock<IFulfillmentService> mockService;
        private IMessagePactBuilderV4 pact;

        [SetUp]
        public void SetUp()
        {
            this.mockService = new Mock<IFulfillmentService>();
            this.consumer = new OrderCreatedConsumer(this.mockService.Object);

            var config = new PactConfig
            {
                PactDir = Path.Join("..", "..", "..", "pacts"),
                DefaultJsonSettings = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                }
            };

            this.pact = Pact.V4("fulfillment_consumer", "order_provider", config).WithMessageInteractions();
        }

        [Test]
        public async Task OnMessageAsync_OrderCreated_HandlesMessage()
        {
            await this.pact
                .ExpectsToReceive("an event indicating that an order has been created")
                .WithJsonContent(new
                {
                    Id = Match.Integer(1)
                })
                .VerifyAsync<OrderCreatedEvent>(async message =>
                {
                    await this.consumer.OnMessageAsync(message);

                    this.mockService.Verify(s => s.FulfillOrderAsync(message.id));
                });
        }
    }
}
