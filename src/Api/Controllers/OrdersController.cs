using Application.Queries.Order;
using CustomerOrderManagement.Application.Commands.Order;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Controller for managing orders
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderCommandHandler _commandHandler;
        private readonly OrderQueryHandler _queryHandler;

        public OrdersController(
            OrderCommandHandler commandHandler,
            OrderQueryHandler queryHandler)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
        }

        /// <summary>
        /// Gets an order by ID
        /// </summary>
        /// <param name="id">The order ID</param>
        /// <returns>Order details</returns>
        /// <response code="200">Returns the order</response>
        /// <response code="404">Order not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _queryHandler.HandleAsync(new GetOrderByIdQuery { Id = id });

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        /// <summary>
        /// Gets customer orders filtered by date range
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <param name="startDate">Optional start date filter</param>
        /// <param name="endDate">Optional end date filter</param>
        /// <returns>List of orders ordered by date</returns>
        /// <response code="200">Returns the list of orders</response>
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetCustomerOrdersByDate(
            Guid customerId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var orders = await _queryHandler.HandleAsync(new GetCustomerOrdersByDateQuery
            {
                CustomerId = customerId,
                StartDate = startDate,
                EndDate = endDate
            });

            return Ok(orders);
        }
    }
}