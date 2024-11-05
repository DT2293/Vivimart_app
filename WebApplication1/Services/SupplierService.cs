using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using X.PagedList.Extensions;
using X.PagedList;

namespace WebApplication1.Services
{
    public class SupplierService
    {
        private readonly AuthContext _db;

        public SupplierService(AuthContext db)
        {
            _db = db;
        }

        //public List<Suppliers> GetAll()
        //{
        //    var obj = _db.Suppliers.ToList();

        //    return obj;
        //}
        public IPagedList<Suppliers> GetAll(int pageNumber, int pageSize)
        {
            return _db.Suppliers.ToPagedList(pageNumber, pageSize);
        }
        public Suppliers GetById(int id)
        {
            var obj = _db.Suppliers.Where(x => x.SupplierID == id).FirstOrDefault();

            return obj;
        }
        public async Task<bool> InsertSupplierAsync(Suppliers supplier)
        {
            try
            {
                _db.Suppliers.Add(supplier);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                // Xử lý hoặc ghi lại ngoại lệ nếu cần
                Console.WriteLine($"Error inserting supplier: {ex.InnerException?.Message}");
                return false;
            }
        }
        public bool UpdateSupplier(Suppliers supplier)
        {
            try
            {
                _db.Suppliers.Update(supplier);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Updating supplier: {ex.Message}");
                return false;
            }
        }
        public bool DeleteSupplier(int id)
        {
            try
            {
                var supplier = _db.Suppliers.Find(id);
                if (supplier == null) return false;

                _db.Suppliers.Remove(supplier);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Deleting supplier: {ex.Message}");
                return false;
            }
        }

        public List<Suppliers> GetByName(string name)
        {
            List<Suppliers> obj = _db.Suppliers.Where(x => (x.SupplierName.Contains(name))).ToList();
                           
            return obj;
        }

        //public OrderProduct GetOrderWithDetails(int supplierId)
        //{
        //    var order = _db.OrderProducts
        //        .Include(o => o.OrderProductDetails)
        //        .FirstOrDefault(o => o.SupplierID == supplierId);

        //    return order;
        //}


    }
}
