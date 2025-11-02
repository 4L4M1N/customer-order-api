using Application.Commands.Product;
using Application.Queries.Product;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Controller for managing products
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductCommandHandler _commandHandler;
        private readonly ProductQueryHandler _queryHandler;

        public ProductsController(
            ProductCommandHandler commandHandler,
            ProductQueryHandler queryHandler)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>List of all products</returns>
        /// <response code="200">Returns the list of products</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAll()
        {
            var products = await _queryHandler.HandleAsync(new GetAllProductsQuery());
            return Ok(products);
        }

        /// <summary>
        /// Gets a specific product by ID
        /// </summary>
        /// <param name="id">The product ID</param>
        /// <returns>Product details</returns>
        /// <response code="200">Returns the product</response>
        /// <response code="404">Product not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _queryHandler.HandleAsync(new GetProductByIdQuery { Id = id });

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="command">Product creation data</param>
        /// <returns>The created product ID</returns>
        /// <response code="201">Product created successfully</response>
        /// <response code="400">Invalid input data</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            try
            {
                var productId = await _commandHandler.HandleAsync(command);
                return CreatedAtAction(nameof(GetById), new { id = productId }, new { id = productId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="id">The product ID</param>
        /// <param name="command">Product update data</param>
        /// <returns>No content</returns>
        /// <response code="204">Product updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Product not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
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
        /// Deletes a product
        /// </summary>
        /// <param name="id">The product ID</param>
        /// <returns>No content</returns>
        /// <response code="204">Product deleted successfully</response>
        /// <response code="404">Product not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _commandHandler.HandleAsync(new DeleteProductCommand { Id = id });
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
    }
}