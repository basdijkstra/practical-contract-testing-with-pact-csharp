using OrderProvider.Orders;
using OrderProvider.PubSub;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace OrderProvider
{
    public class Startup
    {
        public IConfiguration? Configuration { get; }

        public Startup(IConfiguration? configuration)
        {
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddTransient<IMessagePublisher, MessagePublisher>();
            services.TryAddSingleton<IOrderRepository, OrderRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}
