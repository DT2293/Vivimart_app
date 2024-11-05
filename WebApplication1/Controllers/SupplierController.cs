using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;
using X.PagedList.Extensions;

namespace WebApplication1.Controllers
{
    public class SupplierController : Controller
    {
        private readonly SupplierService _supplierService;
        private readonly EmailService _emailService;

        public SupplierController(SupplierService supplierService, EmailService emailService)
        {
            _supplierService = supplierService;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult OrderProductPage(int id)
        {
            // Lấy thông tin nhà cung cấp từ dịch vụ
            var supplier = _supplierService.GetById(id);

            if (supplier == null)
            {
                // Nếu không tìm thấy nhà cung cấp, có thể điều hướng về trang trước đó hoặc hiển thị thông báo lỗi
                return RedirectToAction("SupplierPage");
            }

            // Trả về view với model đã được điền thông tin
            return View(new OrderRequestViewModel
            {
                SupplierId = supplier.SupplierID,
                SupplierName = supplier.SupplierName, // Lấy tên nhà cung cấp từ đối tượng supplier
                Quantity = 0, // Hoặc giá trị mặc định bạn muốn
                Message = string.Empty // Hoặc giá trị mặc định bạn muốn
            });
        }
        [HttpGet]
        public IActionResult InsertSupplier()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InsertSupplier(Suppliers supplier)
        {
            if (ModelState.IsValid)
            {
                bool success = await _supplierService.InsertSupplierAsync(supplier);
                if (success)
                {
                    return RedirectToAction("SupplierPage"); // Redirect to a list page or another page as desired
                }
                ModelState.AddModelError(string.Empty, "An error occurred while saving the supplier.");
            }

            return View(supplier);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var supplier = _supplierService.GetById(id);
            if (supplier == null)
            {
                return NotFound();
            }

            var viewModel = new SupplierViewModel
            {
                SupplierID = supplier.SupplierID,
                SupplierName = supplier.SupplierName,
                ContactName = supplier.ContactName,
                Address = supplier.Address,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Fax = supplier.Fax
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, SupplierViewModel viewModel)
        {
            if (id != viewModel.SupplierID)
            {
                return BadRequest("Supplier ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                var supplier = new Suppliers
                {
                    SupplierID = viewModel.SupplierID,
                    SupplierName = viewModel.SupplierName,
                    ContactName = viewModel.ContactName,
                    Address = viewModel.Address,
                    Phone = viewModel.Phone,
                    Email = viewModel.Email,
                    Fax = viewModel.Fax
                };

                var result = _supplierService.UpdateSupplier(supplier);

                if (result)
                {
                    TempData["SuccessMessage"] = "Supplier updated successfully.";
                    return RedirectToAction("SupplierPage");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update supplier. Please try again.");
                }
            }

            return View(viewModel);
        }

        public IActionResult Delete(int id)
        {
            _supplierService.DeleteSupplier(id);

            return RedirectToAction("SupplierPage");
        }
        //[HttpPost]
        //public async Task<IActionResult> Edit(int id, Suppliers supplier)
        //{
        //    if (id != supplier.Id)
        //    {
        //        return BadRequest("Supplier ID mismatch.");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var result = await _suppliersService.UpdateSupplierAsync(supplier);

        //        if (result)
        //        {
        //            TempData["SuccessMessage"] = "Supplier updated successfully.";
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Failed to update supplier. Please try again.");
        //        }
        //    }

        //    return View(supplier); // Trả về view với dữ liệu lỗi để người dùng sửa lại
        //}
        //public IActionResult InsertSupplier(SupplierViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var result =  _supplierService.Insert(model.suppliers);
        //        if (result)
        //        {
        //            return RedirectToAction("SupplierPage");
        //        }
        //        ModelState.AddModelError("", "Không thể thêm nhà cung cấp.");
        //    }
        //    return View(model);
        //}

        [HttpPost]
        public async Task<IActionResult> SendOrderRequest(OrderRequestViewModel model)
        {
            // Kiểm tra tính hợp lệ của dữ liệu
            if (!ModelState.IsValid)
            {
                return View("OrderProductPage", model); // Trả về OrderProductPage với đúng model
            }

            // Lấy thông tin nhà cung cấp
            var supplier = _supplierService.GetById(model.SupplierId);
            if (supplier == null)
            {
                ModelState.AddModelError("", "Supplier not found.");
                return View("OrderProductPage", model); // Sửa tên view thành OrderProductPage khi không tìm thấy nhà cung cấp
            }

            // Gửi email yêu cầu đặt hàng
            string emailContent = $"<p>Dear {supplier.SupplierName},</p>"
                                + $"<p>We would like to place an order with the following details:</p>"
                                + $"<p>Product: {model.SupplierName}<br>"
                                + $"Quantity: {model.Quantity}<br>"
                                + $"Message: {model.Message}</p>";

            await _emailService.SendEmailAsync(supplier.Email, "Order Request", emailContent);

            ViewBag.Message = "Order request has been sent successfully!";
            return RedirectToAction("SupplierPage"); // Điều hướng về SupplierPage sau khi gửi thành công
        }

      
        public IActionResult SupplierPage(string keyWord, int? page)
        {
            int pageSize = 7; // Số lượng mục trên mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại

            var model = new SupplierViewModel();
            IPagedList<Suppliers> suppliers;

            if (string.IsNullOrEmpty(keyWord))
            {
                suppliers = _supplierService.GetAll(pageNumber, pageSize);
            }
            else
            {
                suppliers = _supplierService.GetByName(keyWord).ToPagedList(pageNumber, pageSize);
                model.keyWord = keyWord;
            }

            model.listSupplier = suppliers;
            return View(model);
        }

    }
}
