using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Services.SystemService
{
    public class ProductCategoryService
    {
        private readonly AuthContext _db;

        public ProductCategoryService(AuthContext db)
        {
            _db = db;
        }

     

        public async Task<bool> InsertCategoryAsync(string categoryName, string username)
        {
            // Lấy UserId dựa trên Username
            var userInfo = await _db.Users
                .Where(u => u.Username == username)
                .Select(u => new { u.UserId })
                .FirstOrDefaultAsync();

            if (userInfo == null)
            {
                // Không tìm thấy người dùng
                return false;
            }

            // Tạo mới ProductCategory
            var productCategory = new ProductCategory
            {
                CategoryName = categoryName,
                UserId = userInfo.UserId
            };

            // Thêm vào cơ sở dữ liệu và lưu thay đổi
            _db.ProductCategories.Add(productCategory);
            await _db.SaveChangesAsync();

            return true;
        }


        public bool Delete(int id)
        {
            var rs = _db.ProductCategories.Where(x => x.CategoryId == id).FirstOrDefault();

            if (rs != null)
            {
                _db.ProductCategories.Remove(rs);
                _db.SaveChanges();
            }

            return true;
        }
        public List<ProductCategory> GetAll()
        {
            var obj = _db.ProductCategories.ToList();

            return obj;
        }

        public ProductCategory GetById(int id)
        {
            var obj = _db.ProductCategories.Where(x => x.CategoryId == id).FirstOrDefault();

            return obj;
        }

        public List<ProductCategory> GetByName(string name)
        {
            List<ProductCategory> obj = _db.ProductCategories.Where(x => x.CategoryName.Contains(name)).ToList();

            return obj;
        }
        
    }
}
