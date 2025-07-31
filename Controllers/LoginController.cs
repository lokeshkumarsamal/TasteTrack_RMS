using Microsoft.AspNetCore.Mvc;
using TasteTrack_RMS.Models;
using TasteTrack_RMS.Repositories;

namespace TasteTrack_RMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<LoginController> _logger;

        public LoginController(IUserRepository userRepository, ILogger<LoginController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid request data"
                    });
                }

                var result = await _userRepository.AuthenticateAsync(request.UserId, request.Password);

                if (result.Success)
                {
                    _logger.LogInformation($"User {request.UserId} logged in successfully");
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning($"Failed login attempt for user {request.UserId}");
                    return Unauthorized(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login process");
                return StatusCode(500, new LoginResponse
                {
                    Success = false,
                    Message = "An error occurred during login"
                });
            }
        }

        [HttpGet("validate/{userid}")]
        public async Task<ActionResult<usermaster>> ValidateUser(string userid)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userid);
                if (user == null)
                {
                    return NotFound($"User {userid} not found");
                }

                // Don't return password in response
                user.passwd = string.Empty;
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating user {userid}");
                return StatusCode(500, "Error validating user");
            }
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(request.UserId);
                if (user == null)
                {
                    return Unauthorized(new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid user"
                    });
                }

                var result = await _userRepository.AuthenticateAsync(request.UserId, user.passwd);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return StatusCode(500, new LoginResponse
                {
                    Success = false,
                    Message = "Error refreshing token"
                });
            }
        }
    }

    public class RefreshTokenRequest
    {
        public string UserId { get; set; } = string.Empty;
    }
}
