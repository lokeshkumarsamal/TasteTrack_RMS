using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasteTrack_RMS.Models;
using TasteTrack_RMS.Repositories;

namespace TasteTrack_RMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository _reportRepository;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportRepository reportRepository, ILogger<ReportController> logger)
        {
            _reportRepository = reportRepository;
            _logger = logger;
        }

        [HttpGet("daily-itemwise")]
        public async Task<ActionResult<List<ItemWiseReportData>>> GetDailyItemWiseReport(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var report = await _reportRepository.GetDailyItemWiseReportAsync(startDate, endDate);
                return Ok(new ApiResponse<List<ItemWiseReportData>>
                {
                    Success = true,
                    Data = report,
                    Message = "Daily item-wise report retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving daily item-wise report");
                return StatusCode(500, new ApiResponse<List<ItemWiseReportData>>
                {
                    Success = false,
                    Message = "Error retrieving daily item-wise report"
                });
            }
        }

        [HttpGet("daily-orderwise")]
        public async Task<ActionResult<List<SalesMaster>>> GetDailyOrderWiseReport(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var report = await _reportRepository.GetDailyOrderWiseReportAsync(startDate, endDate);
                return Ok(new ApiResponse<List<SalesMaster>>
                {
                    Success = true,
                    Data = report,
                    Message = "Daily order-wise report retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving daily order-wise report");
                return StatusCode(500, new ApiResponse<List<SalesMaster>>
                {
                    Success = false,
                    Message = "Error retrieving daily order-wise report"
                });
            }
        }

        [HttpGet("sales")]
        public async Task<ActionResult<List<SalesSlave>>> GetSalesReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate == default || endDate == default)
                {
                    return BadRequest(new ApiResponse<List<SalesSlave>>
                    {
                        Success = false,
                        Message = "Start date and end date are required"
                    });
                }

                if (startDate > endDate)
                {
                    return BadRequest(new ApiResponse<List<SalesSlave>>
                    {
                        Success = false,
                        Message = "Start date cannot be greater than end date"
                    });
                }

                var report = await _reportRepository.GetSalesReportAsync(startDate, endDate);
                return Ok(new ApiResponse<List<SalesSlave>>
                {
                    Success = true,
                    Data = report,
                    Message = "Sales report retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sales report");
                return StatusCode(500, new ApiResponse<List<SalesSlave>>
                {
                    Success = false,
                    Message = "Error retrieving sales report"
                });
            }
        }

        [HttpGet("sales-comparison")]
        public async Task<ActionResult<List<DailySalesReport>>> GetSalesComparisonReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate == default || endDate == default)
                {
                    return BadRequest(new ApiResponse<List<DailySalesReport>>
                    {
                        Success = false,
                        Message = "Start date and end date are required"
                    });
                }

                if (startDate > endDate)
                {
                    return BadRequest(new ApiResponse<List<DailySalesReport>>
                    {
                        Success = false,
                        Message = "Start date cannot be greater than end date"
                    });
                }

                var report = await _reportRepository.GetSalesComparisonReportAsync(startDate, endDate);
                return Ok(new ApiResponse<List<DailySalesReport>>
                {
                    Success = true,
                    Data = report,
                    Message = "Sales comparison report retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sales comparison report");
                return StatusCode(500, new ApiResponse<List<DailySalesReport>>
                {
                    Success = false,
                    Message = "Error retrieving sales comparison report"
                });
            }
        }

        [HttpGet("dashboard-summary")]
        public async Task<ActionResult<DashboardSummary>> GetDashboardSummary()
        {
            try
            {
                var today = DateTime.Today;

                // Get today's sales
                var todaySales = await _reportRepository.GetDailyOrderWiseReportAsync(today, today);

                // Get this month's sales comparison
                var monthStart = new DateTime(today.Year, today.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                var monthlySales = await _reportRepository.GetSalesComparisonReportAsync(monthStart, monthEnd);

                var summary = new DashboardSummary
                {
                    TodayTotalSales = todaySales.Sum(s => s.TotalValue ?? 0),
                    TodayTotalOrders = todaySales.Count,
                    MonthlyTotalSales = monthlySales.Sum(s => s.TotalValue), // <- Fixed line 181
                    MonthlyTotalOrders = monthlySales.Count,
                    LastUpdated = DateTime.Now
                };

                return Ok(new ApiResponse<DashboardSummary>
                {
                    Success = true,
                    Data = summary,
                    Message = "Dashboard summary retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard summary");
                return StatusCode(500, new ApiResponse<DashboardSummary>
                {
                    Success = false,
                    Message = "Error retrieving dashboard summary"
                });
            }
        }
    }

    public class DashboardSummary
    {
        public decimal TodayTotalSales { get; set; }
        public int TodayTotalOrders { get; set; }
        public decimal MonthlyTotalSales { get; set; }
        public int MonthlyTotalOrders { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
