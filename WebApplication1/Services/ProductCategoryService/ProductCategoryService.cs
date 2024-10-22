using WebApplication1.Models;

namespace WebApplication1.Services.SystemService
{
    public class ProductCategoryService
    {
        public readonly AuthContext _db;

        public ProductCategoryService(AuthContext db)
        {
            _db = db; 
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

        public bool InsertCategory(ProductCategory category)
        {
            _db.Add(category);
            _db.SaveChanges();

            return true;

        }

        public bool Update(ProductCategory category)
        {
            var rs= _db.ProductCategories.Where(x => x.CategoryId == category.CategoryId).FirstOrDefault();

            if (rs != null)
            {
                rs.CategoryName = category.CategoryName;
                _db.SaveChanges();
                return true;
            }

            return false;
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
    }
}
