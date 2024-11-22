using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebApplication1.Models;
using WebApplication1.Services;

public class SaleRecordController : Controller
{
    private readonly RevenueStatisticsService _revenueStatisticsService;
    private readonly AuthContext _db;

    public SaleRecordController(RevenueStatisticsService revenueStatisticsService, AuthContext db)
    {
        _revenueStatisticsService = revenueStatisticsService;
        _db = db;
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

    [HttpGet]
    public async Task<JsonResult> getMonthlySaleData()
    {
        var salesData = await _revenueStatisticsService.GetMonthlyRevenueStatisticsAsync();
        var result = salesData.Select(s => new
        {
            Nam = s.Nam,  // Chỉ cần năm dưới dạng số nguyên
            Thang = s.Thang.ToString("00"),  // Đảm bảo tháng có 2 chữ số
            tongDoanhThu = s.TongDoanhThu
        }).ToList();

        return Json(result);
    }


    [HttpGet]
    public IActionResult TopSellingProducts()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> TopSellingProducts(DateTime startDate, DateTime endDate, int top = 10)
    {
        var topSellingProducts = await _revenueStatisticsService.GetTopSellingProductsAsync(top, startDate, endDate);
        return View(topSellingProducts);
    }




}
