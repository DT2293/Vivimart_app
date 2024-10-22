using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

public class SaleRecordController : Controller
{
    private readonly RevenueStatisticsService _revenueStatisticsService;

    public SaleRecordController(RevenueStatisticsService revenueStatisticsService)
    {
        _revenueStatisticsService = revenueStatisticsService;
    }

    public IActionResult ShowChart()
    {
        return View();
    }

    [HttpGet]
    public async Task<JsonResult> GetSaleData()
    {
        var salesData = await _revenueStatisticsService.GetDailyRevenueStatisticsAsync();
        var result = salesData.Select(s => new
        {
            ngay = s.Ngay.ToString("dd/MM/yyyy"),
            tongDoanhThu = s.TongDoanhThu
        }).ToList();

        return Json(result);
    }
}
