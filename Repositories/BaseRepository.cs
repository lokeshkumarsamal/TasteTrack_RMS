using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace TasteTrack_RMS.Repositories
{
    public class BaseRepository
    {
        private readonly string _connectionString;
        protected readonly ILogger _logger;

        public BaseRepository(IConfiguration configuration, ILogger logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentException("Connection string not found");
            _logger = logger;
        }

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        protected async Task<T> ExecuteWithExceptionHandlingAsync<T>(Func<Task<T>> operation, string operationName)
        {
            try
            {
                _logger.LogInformation($"Starting {operationName}");
                var result = await operation();
                _logger.LogInformation($"Completed {operationName} successfully");
                return result;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, $"SQL Error in {operationName}: {ex.Message}");
                throw new Exception($"Database error occurred in {operationName}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {operationName}: {ex.Message}");
                throw new Exception($"An error occurred in {operationName}", ex);
            }
        }
    }
}
