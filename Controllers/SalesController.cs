using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasteTrack_RMS.Models;
using TasteTrack_RMS.Repositories;

namespace TasteTrack_RMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SalesController : ControllerBase
    {
        private readonly ISalesRepository _salesRepository;
        private readonly ILogger<SalesController> _logger;

        public SalesController(ISalesRepository salesRepository, ILogger<SalesController> logger)
        {
            _salesRepository = salesRepository;
            _logger = logger;
        }

        [HttpPost("add-item")]
        [Authorize(Roles = "user,admin")]
        public async Task<ActionResult> AddItemToSale([FromBody] AddItemRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                    });
                }

                if (request.Quantity <= 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Quantity must be greater than 0"
                    });
                }

                var result = await _salesRepository.AddItemToSaleAsync(request.ItemId, request.Quantity);
                if (result)
                {
                    _logger.LogInformation($"Item {request.ItemId} (qty: {request.Quantity}) added to sale successfully");
                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Item added to sale successfully"
                    });
                }

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Failed to add item to sale"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to sale");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error adding item to sale"
                });
            }
        }

        [HttpDelete("remove-item/{itemId}")]
        [Authorize(Roles = "user,admin")]
        public async Task<ActionResult> RemoveItemFromSale(int itemId)
        {
            try
            {
                var result = await _salesRepository.RemoveItemFromSaleAsync(itemId);
                if (result)
                {
                    _logger.LogInformation($"Item {itemId} removed from sale successfully");
                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Item removed from sale successfully"
                    });
                }

                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Item not found in current sale"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing item {itemId} from sale");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error removing item from sale"
                });
            }
        }

        [HttpPost("complete")]
        [Authorize(Roles = "user,admin")]
        public async Task<ActionResult> CompleteSale()
        {
            try
            {
                var result = await _salesRepository.CompleteSaleAsync();
                if (result)
                {
                    _logger.LogInformation("Sale completed successfully");
                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Sale completed successfully"
                    });
                }

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Failed to complete sale"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing sale");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error completing sale"
                });
            }
        }

        [HttpGet("current")]
        [Authorize(Roles = "user,admin")]
        public async Task<ActionResult<List<SalesSlave>>> GetCurrentSaleItems()
        {
            try
            {
                var items = await _salesRepository.GetCurrentSaleItemsAsync();
                return Ok(new ApiResponse<List<SalesSlave>>
                {
                    Success = true,
                    Data = items,
                    Message = "Current sale items retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current sale items");
                return StatusCode(500, new ApiResponse<List<SalesSlave>>
                {
                    Success = false,
                    Message = "Error retrieving current sale items"
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<SalesMaster>>> GetAllSales()
        {
            try
            {
                var sales = await _salesRepository.GetAllSalesAsync();
                return Ok(new ApiResponse<List<SalesMaster>>
                {
                    Success = true,
                    Data = sales,
                    Message = "Sales retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all sales");
                return StatusCode(500, new ApiResponse<List<SalesMaster>>
                {
                    Success = false,
                    Message = "Error retrieving sales"
                });
            }
        }

        [HttpGet("{orderId}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<SalesMaster>> GetSale(int orderId)
        {
            try
            {
                var sale = await _salesRepository.GetSaleByIdAsync(orderId);
                if (sale == null)
                {
                    return NotFound(new ApiResponse<SalesMaster>
                    {
                        Success = false,
                        Message = $"Sale with ID {orderId} not found"
                    });
                }

                return Ok(new ApiResponse<SalesMaster>
                {
                    Success = true,
                    Data = sale,
                    Message = "Sale retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving sale {orderId}");
                return StatusCode(500, new ApiResponse<SalesMaster>
                {
                    Success = false,
                    Message = "Error retrieving sale"
                });
            }
        }

        [HttpGet("{orderId}/items")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<SalesSlave>>> GetSaleItems(int orderId)
        {
            try
            {
                var items = await _salesRepository.GetSaleItemsAsync(orderId);
                return Ok(new ApiResponse<List<SalesSlave>>
                {
                    Success = true,
                    Data = items,
                    Message = "Sale items retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving items for sale {orderId}");
                return StatusCode(500, new ApiResponse<List<SalesSlave>>
                {
                    Success = false,
                    Message = "Error retrieving sale items"
                });
            }
        }
    }

    public class AddItemRequest
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
}
