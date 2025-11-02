using CustomerOrderManagement.Application.Commands.ShoppingCart;
using CustomerOrderManagement.Application.Queries.ShoppingCart;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CustomerOrderManagement.Api.Controllers
{
    /// <summary>
    /// Controller for managing shopping carts
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly ShoppingCartCommandHandler _commandHandler;
        private readonly ShoppingCartQueryHandler _queryHandler;

        public ShoppingCartController(
            ShoppingCartCommandHandler commandHandler,
            ShoppingCartQueryHandler queryHandler)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
        }

        /// <summary>
        /// Gets the shopping cart for a customer
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <returns>Shopping cart with items</returns>
        /// <response code="200">Returns the shopping cart</response>
        /// <response code="404">Shopping cart not found (cart is empty or doesn't exist)</response>
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCart(Guid customerId)
        {
            var cart = await _queryHandler.HandleAsync(new ShoppingCartQueries.GetShoppingCartQuery { CustomerId = customerId });

            if (cart == null)
                return NotFound(new { message = "Shopping cart not found or is empty" });

            return Ok(cart);
        }

        /// <summary>
        /// Adds an item to the shopping cart
        /// </summary>
        /// <param name="command">Cart item data</param>
        /// <returns>No content</returns>
        /// <response code="204">Item added to cart successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Product not found</response>
        [HttpPost("items")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddToCart([FromBody] ShoppingCartCommands.AddToCartCommand command)
        {
            try
            {
                await _commandHandler.HandleAsync(command);
                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Updates the quantity of a cart item
        /// </summary>
        /// <param name="command">Cart item update data</param>
        /// <returns>No content</returns>
        /// <response code="204">Cart item updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Cart or item not found</response>
        [HttpPut("items")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCartItem([FromBody] ShoppingCartCommands.UpdateCartItemCommand command)
        {
            try
            {
                await _commandHandler.HandleAsync(command);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Removes an item from the shopping cart
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <param name="productId">The product ID to remove</param>
        /// <returns>No content</returns>
        /// <response code="204">Item removed successfully</response>
        /// <response code="404">Cart not found</response>
        [HttpDelete("customer/{customerId}/items/{productId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RemoveFromCart(Guid customerId, Guid productId)
        {
            try
            {
                await _commandHandler.HandleAsync(new ShoppingCartCommands.RemoveFromCartCommand
                {
                    CustomerId = customerId,
                    ProductId = productId
                });
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Clears all items from the shopping cart
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <returns>No content</returns>
        /// <response code="204">Cart cleared successfully</response>
        [HttpDelete("customer/{customerId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> ClearCart(Guid customerId)
        {
            await _commandHandler.HandleAsync(new ShoppingCartCommands.ClearCartCommand { CustomerId = customerId });
            return NoContent();
        }

        /// <summary>
        /// Checkout - converts shopping cart to order and clears the cart
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <returns>The created order ID</returns>
        /// <response code="201">Order created successfully from cart</response>
        /// <response code="400">Cart is empty or invalid</response>
        /// <response code="404">Cart not found</response>
        [HttpPost("customer/{customerId}/checkout")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Checkout(Guid customerId)
        {
            try
            {
                var orderId = await _commandHandler.HandleAsync(new ShoppingCartCommands.CheckoutCommand { CustomerId = customerId });
                return CreatedAtAction(
                    actionName: "GetById",
                    controllerName: "Orders",
                    routeValues: new { id = orderId },
                    value: new { orderId, message = "Order created successfully. Shopping cart has been cleared." }
                );
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("empty"))
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}