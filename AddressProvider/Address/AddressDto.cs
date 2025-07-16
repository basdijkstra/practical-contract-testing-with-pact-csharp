namespace AddressProvider.Address
{
    public class AddressDto
    {
        public string Id { get; set; } = string.Empty;
        public string AddressType { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public int Number { get; set; }
        public int PoBox {  get; set; }
        public string City { get; set; } = string.Empty;
        public int ZipCode { get; set; }
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
