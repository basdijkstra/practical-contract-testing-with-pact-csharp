using AddressProvider.Address;
using Microsoft.AspNetCore.Mvc;

namespace AddressProvider.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressRepository addresses;
        private readonly ILogger<AddressController> _logger;

        public AddressController(IAddressRepository addresses, ILogger<AddressController> logger)
        {
            this.addresses = addresses;
            _logger = logger;
        }

        [HttpGet("{id}", Name = "GET address by ID")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(string id)
        {
            _logger.LogInformation("GET address with ID {}", id);

            if (!Guid.TryParse(id, out Guid result))
            {
                return BadRequest();
            }

            AddressDto address = await this.addresses.GetAddressByIdAsync(id);

            if (address is null)
            {
                return NotFound();
            }

            return Ok(address);

            //try
            //{
            //    AddressDto address = await this.addresses.GetAddressByIdAsync(id);
            //    return Ok(address);
            //}
            //catch (KeyNotFoundException)
            //{
            //    return NotFound();
            //}
        }

        [HttpPost(Name = "POST a new address")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody] AddressDto address)
        {
            _logger.LogInformation("POST address with ID {}", address.Id);

            await this.addresses.AddAddressAsync(address);

            return this.CreatedAtRoute("GET address by ID", new { Id = address.Id }, address);
        }

        [HttpDelete("{id}", Name = "DELETE an address by ID")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!Guid.TryParse(id, out Guid result))
            {
                return BadRequest();
            }

            await this.addresses.DeleteAddressAsync(id);
            return NoContent();
        }
    }
}
