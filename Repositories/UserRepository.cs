using Dapper;
using System.Data;
using TasteTrack_RMS.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TasteTrack_RMS.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration, ILogger<UserRepository> logger)
            : base(configuration, logger)
        {
            _configuration = configuration;
        }

        public async Task<LoginResponse> AuthenticateAsync(string userid, string password)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@userid", userid);
                parameters.Add("@passwd", password);
                parameters.Add("@usertype", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);

                await connection.ExecuteAsync("sp_rms_login", parameters, commandType: CommandType.StoredProcedure);

                var userType = parameters.Get<string>("@usertype");

                if (userType == "Fail" || string.IsNullOrEmpty(userType))
                {
                    _logger.LogWarning($"Failed login attempt for user: {userid}");
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid credentials"
                    };
                }

                var token = GenerateJwtToken(userid, userType);
                _logger.LogInformation($"User {userid} authenticated successfully with role {userType}");

                return new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    UserType = userType,
                    UserId = userid
                };
            }, "User Authentication");
        }

        public async Task<usermaster?> GetUserByIdAsync(string userid)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var user = await connection.QueryFirstOrDefaultAsync<usermaster>(
                    "SELECT userid, passwd, usertype FROM UserMaster WHERE userid = @userid",
                    new { userid });
                return user;
            }, "Get User By ID");
        }

        public async Task<List<usermaster>> GetAllUsersAsync()
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var users = await connection.QueryAsync<usermaster>(
                    "SELECT userid, passwd, usertype FROM UserMaster ORDER BY userid");
                return users.ToList();
            }, "Get All Users");
        }

        public async Task<bool> CreateUserAsync(usermaster user)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();

                // Check if user already exists
                var existingUser = await connection.QueryFirstOrDefaultAsync<usermaster>(
                    "SELECT userid FROM UserMaster WHERE userid = @userid",
                    new { userid = user.userid });

                if (existingUser != null)
                {
                    _logger.LogWarning($"Attempt to create duplicate user: {user.userid}");
                    return false;
                }

                var result = await connection.ExecuteAsync(
                    "INSERT INTO UserMaster (userid, passwd, usertype) VALUES (@userid, @passwd, @usertype)",
                    user);

                if (result > 0)
                {
                    _logger.LogInformation($"User {user.userid} created successfully with role {user.usertype}");
                }

                return result > 0;
            }, "Create User");
        }

        public async Task<bool> UpdateUserAsync(usermaster user)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var result = await connection.ExecuteAsync(
                    "UPDATE UserMaster SET passwd = @passwd, usertype = @usertype WHERE userid = @userid",
                    user);

                if (result > 0)
                {
                    _logger.LogInformation($"User {user.userid} updated successfully");
                }

                return result > 0;
            }, "Update User");
        }

        public async Task<bool> DeleteUserAsync(string userid)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();

                // Prevent deleting admin users
                var user = await connection.QueryFirstOrDefaultAsync<usermaster>(
                    "SELECT usertype FROM UserMaster WHERE userid = @userid",
                    new { userid });

                if (user?.usertype == "admin")
                {
                    _logger.LogWarning($"Attempt to delete admin user: {userid}");
                    return false;
                }

                var result = await connection.ExecuteAsync(
                    "DELETE FROM UserMaster WHERE userid = @userid",
                    new { userid });

                if (result > 0)
                {
                    _logger.LogInformation($"User {userid} deleted successfully");
                }

                return result > 0;
            }, "Delete User");
        }

        public async Task<bool> UserExistsAsync(string userid)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var count = await connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(*) FROM UserMaster WHERE userid = @userid",
                    new { userid });
                return count > 0;
            }, "Check User Exists");
        }

        private string GenerateJwtToken(string userid, string usertype)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"];

                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new InvalidOperationException("JWT SecretKey not configured");
                }

                var key = Encoding.ASCII.GetBytes(secretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("userid", userid),
                        new Claim("usertype", usertype),
                        new Claim(ClaimTypes.Role, usertype),
                        new Claim(ClaimTypes.Name, userid),
                        new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(jwtSettings["ExpiryMinutes"])),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature),
                    Issuer = jwtSettings["Issuer"],
                    Audience = jwtSettings["Audience"]
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                _logger.LogInformation($"JWT token generated for user {userid} with role {usertype}");
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating JWT token for user {userid}");
                throw new InvalidOperationException("Failed to generate authentication token", ex);
            }
        }
    }
}
