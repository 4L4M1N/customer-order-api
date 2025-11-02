using Application.Queries.Customer;
using CustomerOrderManagement.Application.Commands.Customer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Controller for managing customers
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerCommandHandler _commandHandler;
        private readonly CustomerQueryHandler _queryHandler;

        public CustomersController(
            CustomerCommandHandler commandHandler,
            CustomerQueryHandler queryHandler)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
        }

        /// <summary>
        /// Get all customers. Set includeDeleted=true to include soft-deleted customers.
        /// </summary>
        /// <param name="includeDeleted">Include soft-deleted customers if true</param>
        /// <returns>List of customers</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
        {
            var query = new GetAllCustomersQuery
            {
                IncludeDeleted = includeDeleted
            };
            var customers = await _queryHandler.HandleAsync(query);
            return Ok(customers);
        }

        /// <summary>
        /// Gets a specific customer by ID
        /// </summary>
        /// <param name="id">The customer ID</param>
        /// <returns>Customer details</returns>
        /// <response code="200">Returns the customer</response>
        /// <response code="404">Customer not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var customer = await _queryHandler.HandleAsync(new GetCustomerByIdQuery { Id = id });

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        /// <summary>
        /// Gets a customer with all their orders
        /// </summary>
        /// <param name="id">The customer ID</param>
        /// <returns>Customer with orders</returns>
        /// <response code="200">Returns the customer with orders</response>
        /// <response code="404">Customer not found</response>
        [HttpGet("{id}/with-orders")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetWithOrders(Guid id)
        {
            var customer = await _queryHandler.HandleAsync(new GetCustomerWithOrdersQuery { Id = id });

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        /// <summary>
        /// Creates a new customer
        /// </summary>
        /// <param name="command">Customer creation data</param>
        /// <returns>The created customer ID</returns>
        /// <response code="201">Customer created successfully</response>
        /// <response code="400">Invalid input data</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
        {
            try
            {
                var customerId = await _commandHandler.HandleAsync(command);
                return CreatedAtAction(nameof(GetById), new { id = customerId }, new { id = customerId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing customer
        /// </summary>
        /// <param name="id">The customer ID</param>
        /// <param name="command">Customer update data</param>
        /// <returns>No content</returns>
        /// <response code="204">Customer updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Customer not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerCommand command)
        {
            try
            {
                command.Id = id;
                await _commandHandler.HandleAsync(command);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a customer
        /// </summary>
        /// <param name="id">The customer ID</param>
        /// <returns>No content</returns>
        /// <response code="204">Customer deleted successfully</response>
        /// <response code="404">Customer not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _commandHandler.HandleAsync(new DeleteCustomerCommand { Id = id });
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
    }
}