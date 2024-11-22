
using Microsoft.Data.SqlClient; // Đảm bảo đã thêm namespace này
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebApplication1.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using WebApplication1.Helper;
using WebApplication1.Helper.DTO;

public class RevenueStatisticsService
{
    private readonly AuthContext _db;

    public RevenueStatisticsService(AuthContext db)
    {
        _db = db;
    }

    // Phương thức lấy thống kê doanh thu hàng ngày
    public async Task<List<DailyRevenueStatisticsDTO>> GetDailyRevenueStatisticsAsync()
    {
        var results = new List<DailyRevenueStatisticsDTO>();

        using (var connection = (SqlConnection)_db.Database.GetDbConnection())
        {
            await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "ThongKeDoanhThuTheoNgay"; // Tên stored procedure
                command.CommandType = CommandType.StoredProcedure;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var statistic = new DailyRevenueStatisticsDTO
                        {
                            Ngay = reader.IsDBNull(reader.GetOrdinal("Ngay")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("Ngay")),
                            TongDoanhThu = reader.IsDBNull(reader.GetOrdinal("TongDoanhThu")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TongDoanhThu"))
                        };
                        results.Add(statistic);
                    }
                }
            }
        }

        return results;
    }
    public async Task<List<TopSellingProduct>> GetTopSellingProductsAsync(int top, DateTime startDate, DateTime endDate)
    {
        var topSellingProducts = new List<TopSellingProduct>();

        var topParam = new SqlParameter("@Top", SqlDbType.Int) { Value = top };
        var startDateParam = new SqlParameter("@StartDate", SqlDbType.DateTime) { Value = startDate };
        var endDateParam = new SqlParameter("@EndDate", SqlDbType.DateTime) { Value = endDate };

        topSellingProducts = await _db.TopSellingProducts
            .FromSqlRaw("EXEC [dbo].[GetTopSellingProducts1] @Top, @StartDate, @EndDate", topParam, startDateParam, endDateParam)
            .ToListAsync();


        return topSellingProducts;

    }
    public async Task<List<MonthlyRevenueStatisticsDTO>> GetMonthlyRevenueStatisticsAsync()
    {
        var results = new List<MonthlyRevenueStatisticsDTO>();

        using (var connection = (SqlConnection)_db.Database.GetDbConnection())
        {
            await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "ThongKeDoanhThuTheoThang"; // Tên stored procedure
                command.CommandType = CommandType.StoredProcedure;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var statistic = new MonthlyRevenueStatisticsDTO
                        {
                            Nam = reader.IsDBNull(reader.GetOrdinal("Nam")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("Nam")),
                            Thang = reader.IsDBNull(reader.GetOrdinal("Thang")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("Thang")),
                            TongDoanhThu = reader.IsDBNull(reader.GetOrdinal("TongDoanhThu")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TongDoanhThu"))
                        };
                        results.Add(statistic);
                    }
                }
            }
        }

        return results;
    }




}
