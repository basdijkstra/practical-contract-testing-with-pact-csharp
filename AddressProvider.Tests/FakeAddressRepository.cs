using AddressProvider.Address;
using System.Collections.Concurrent;

namespace AddressProvider.Tests
{
    public class FakeAddressRepository : IAddressRepository
    {
        private readonly ConcurrentDictionary<string, AddressDto> addresses = new();

        public Task AddAddressAsync(AddressDto address)
        {
            this.addresses[address.Id] = address;
            return Task.CompletedTask;
        }

        public Task DeleteAddressAsync(string id)
        {
            this.addresses[id] = null!;
            return Task.CompletedTask;
        }

        public Task<AddressDto> GetAddressByIdAsync(string id)
        {
            AddressDto address = this.addresses[id];
            return Task.FromResult(address);
        }
    }
}
