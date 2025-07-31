using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasteTrack_RMS.Models;
using TasteTrack_RMS.Repositories;

namespace TasteTrack_RMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ItemController : ControllerBase
    {
        private readonly IItemRepository _itemRepository;
        private readonly ILogger<ItemController> _logger;

        public ItemController(IItemRepository itemRepository, ILogger<ItemController> logger)
        {
            _itemRepository = itemRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<ItemMaster>>> GetAllItems()
        {
            try
            {
                var items = await _itemRepository.GetAllItemsAsync();
                return Ok(new ApiResponse<List<ItemMaster>>
                {
                    Success = true,
                    Data = items,
                    Message = "Items retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all items");
                return StatusCode(500, new ApiResponse<List<ItemMaster>>
                {
                    Success = false,
                    Message = "Error retrieving items"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemMaster>> GetItem(int id)
        {
            try
            {
                var item = await _itemRepository.GetItemByIdAsync(id);
                if (item == null)
                {
                    return NotFound(new ApiResponse<ItemMaster>
                    {
                        Success = false,
                        Message = $"Item with ID {id} not found"
                    });
                }

                return Ok(new ApiResponse<ItemMaster>
                {
                    Success = true,
                    Data = item,
                    Message = "Item retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving item {id}");
                return StatusCode(500, new ApiResponse<ItemMaster>
                {
                    Success = false,
                    Message = "Error retrieving item"
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> CreateItem([FromBody] ItemMaster item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid item data",
                        Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                    });
                }

                var result = await _itemRepository.CreateItemAsync(item);
                if (result)
                {
                    _logger.LogInformation($"Item '{item.ItemName}' created successfully");
                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Item created successfully"
                    });
                }

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Failed to create item"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating item");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error creating item"
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> UpdateItem(int id, [FromBody] ItemMaster item)
        {
            try
            {
                if (id != item.ItemID)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Item ID mismatch"
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid item data",
                        Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                    });
                }

                var result = await _itemRepository.UpdateItemAsync(item);
                if (result)
                {
                    _logger.LogInformation($"Item {id} updated successfully");
                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Item updated successfully"
                    });
                }

                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Item with ID {id} not found"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating item {id}");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error updating item"
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteItem(int id)
        {
            try
            {
                var result = await _itemRepository.DeleteItemAsync(id);
                if (result)
                {
                    _logger.LogInformation($"Item {id} deleted successfully");
                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Item deleted successfully"
                    });
                }

                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Item with ID {id} not found"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting item {id}");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error deleting item"
                });
            }
        }

        [HttpGet("daily")]
        public async Task<ActionResult<List<DailyItem>>> GetDailyItems()
        {
            try
            {
                var items = await _itemRepository.GetDailyItemsAsync();
                return Ok(new ApiResponse<List<DailyItem>>
                {
                    Success = true,
                    Data = items,
                    Message = "Daily items retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving daily items");
                return StatusCode(500, new ApiResponse<List<DailyItem>>
                {
                    Success = false,
                    Message = "Error retrieving daily items"
                });
            }
        }

        [HttpPost("daily/{itemId}")]
        [Authorize(Roles = "user,admin")]
        public async Task<ActionResult> AddDailyItem(int itemId)
        {
            try
            {
                var result = await _itemRepository.AddDailyItemAsync(itemId);
                if (result)
                {
                    _logger.LogInformation($"Item {itemId} added to daily items");
                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Item added to daily menu"
                    });
                }

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Failed to add item to daily menu"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding daily item {itemId}");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error adding daily item"
                });


            }
        }
    }
}