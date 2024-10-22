using Microsoft.Data.SqlClient; // Đảm bảo bạn đã thêm namespace này
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebApplication1.Helper;
using WebApplication1.Models;

public class RevenueStatisticsService
{
    private readonly AuthContext _db;

    public RevenueStatisticsService(AuthContext db)
    {
        _db = db;
    }

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
                            TongDoanhThu = reader.IsDBNull(reader.GetOrdinal("TongDoanhThu")) ? 0 : Convert.ToDecimal(reader.GetInt32(reader.GetOrdinal("TongDoanhThu"))) // Chuyển đổi từ Int sang Decimal
                        };
                        results.Add(statistic);
                    }
                }
            }
        }

        return results;
    }

}
