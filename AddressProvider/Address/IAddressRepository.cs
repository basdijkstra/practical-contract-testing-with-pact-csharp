namespace AddressProvider.Address
{
    public interface IAddressRepository
    {
        Task<AddressDto> GetAddressByIdAsync(string id);

        Task DeleteAddressAsync(string id);

        Task AddAddressAsync(AddressDto address);
    }
}
