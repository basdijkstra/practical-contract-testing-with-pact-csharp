using AddressProvider.Address;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;

namespace AddressProvider.Tests.Middleware
{
    public class ProviderStateMiddleware
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly RequestDelegate _next;
        private readonly IDictionary<string, Func<IDictionary<string, object>, Task>> _providerStates;
        private readonly IAddressRepository _addresses;

        public ProviderStateMiddleware(RequestDelegate next, IAddressRepository addresses)
        {
            _next = next;
            _addresses = addresses;
            _providerStates = new Dictionary<string, Func<IDictionary<string, object>, Task>>
            {
                /**
                 * TODO: Add two more provider state entries and their handler method names:
                 * - One for the provider state "Address does not exist",
                 * - Another for the provider state "No specific state required"
                 */

                ["Address exists"] = CreateAddress,
                ["PO Box address exists"] = CreatePoBoxAddress,
                ["Address does not exist"] = DeleteAddress,
                ["No specific state required"] = DoNothing
            };
        }

        private async Task CreateAddress(IDictionary<string, object> parameters)
        {
            JsonElement id = (JsonElement)parameters["id"];

            AddressDto address = new AddressDto
            {
                Id = id.GetString()!,
                AddressType = "delivery",
                Street = "Main street",
                Number = 123,
                City = "Sun City",
                ZipCode = 90210,
                State = "Yukon",
                Country = "United States"
            };

            await _addresses.AddAddressAsync(address);
        }

        private async Task CreatePoBoxAddress(IDictionary<string, object> parameters)
        {
            JsonElement id = (JsonElement)parameters["id"];

            AddressDto address = new AddressDto
            {
                Id = id.GetString()!,
                AddressType = "billing",
                PoBox = 9876,
                City = "Sun City",
                ZipCode = 90210,
                State = "Yukon",
                Country = "United States"
            };

            await _addresses.AddAddressAsync(address);
        }

        /**
         * TODO: Add a provider state handler method for the provider state "No specific state required".
         * This method has the same method signature as the CreateAddress method.
         * The method body can be left empty as we do not need to do anything for this provider state.
         */
        private async Task DoNothing(IDictionary<string, object> parameters)
        {
        }

        /**
         * TODO: Add a provider state handler method for the provider state "Address does not exist".
         * This method also has the same method signature as the CreateAddress method.
         * The method should:
         * - Read the value for 'id' supplied in the parameters as a JsonElement
         * - Delete the address with that id from the _addresses address repository.
         *   Use the DeleteAddressAsync() method to do this (make sure to await the result!).
         *   Make sure to convert the JsonElement to a string so you can pass it as an argument to DeleteAddressAsync().
         */
        private async Task DeleteAddress(IDictionary<string, object> parameters)
        {
            JsonElement id = (JsonElement)parameters["id"];

            await _addresses.DeleteAddressAsync(id.GetString()!);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!(context.Request.Path.Value?.StartsWith("/provider-states") ?? false))
            {
                await this._next.Invoke(context);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;

            if (context.Request.Method == HttpMethod.Post.ToString())
            {
                string jsonRequestBody;

                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    jsonRequestBody = await reader.ReadToEndAsync();
                }

                try
                {
                    ProviderState? providerState = JsonSerializer.Deserialize<ProviderState>(jsonRequestBody, Options);

                    if (!string.IsNullOrEmpty(providerState?.State))
                    {
                        await this._providerStates[providerState.State].Invoke(providerState.Params);
                        await context.Response.WriteAsync(jsonRequestBody);
                    }
                }
                catch (Exception e)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("Failed to deserialise JSON provider state body:");
                    await context.Response.WriteAsync(jsonRequestBody);
                    await context.Response.WriteAsync(string.Empty);
                    await context.Response.WriteAsync(e.ToString());
                }
            }
        }
    }
}
