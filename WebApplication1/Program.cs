using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using System.Globalization;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Services.Account;
using WebApplication1.Services.SystemService;


var builder = WebApplication.CreateBuilder(args);



string strcnn = builder.Configuration.GetConnectionString("cnn");
builder.Services.AddDbContext<AuthContext>(options => options.UseSqlServer(strcnn));
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SystemService>();
builder.Services.AddScoped<CustomPasswordHasher>();
builder.Services.AddScoped<ProductCategoryService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ProductServiceForS>();
builder.Services.AddScoped<InvoiceService>();
builder.Services.AddScoped<UploadService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<RevenueStatisticsService>();
builder.Services.AddScoped<SupplierService>();
builder.Services.AddTransient<EmailService>();  
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSingleton<IVnPayService, VnPayService>();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session sau 30 phút
//    options.Cookie.HttpOnly = true; // Cookie chỉ được truy cập từ server, bảo mật cao hơn
//    options.Cookie.IsEssential = true; // Cookie này cần thiết cho ứng dụng
//});


//builder.Services.AddSession(cfg =>
//{                    // Đăng ký dịch vụ Session
//    cfg.Cookie.Name = "cart";             // Đặt tên Session - tên này sử dụng ở Browser (Cookie)
//    //cfg.IdleTimeout = new TimeSpan(0, 30, 0);    // Thời gian tồn tại của Session
//});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session sau 30 phút
    options.Cookie.HttpOnly = true; // Cookie chỉ được truy cập từ server, bảo mật cao hơn
    options.Cookie.IsEssential = true; // Cookie này cần thiết cho ứng dụng
    options.Cookie.Name = "cart"; // Đặt tên Session (sử dụng cho Cookie)
});
builder.Services.AddHttpContextAccessor(); // Thêm dòng này

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("vi-VN"),
        new CultureInfo("en-US"),
    };

    options.DefaultRequestCulture = new RequestCulture("vi-VN");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();
app.UseRequestLocalization();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Dùng HSTS cho sản phẩm để tăng cường bảo mật
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();// Kích hoạt Session


app.UseAuthentication();
// Kích hoạt Authentication và Authorization
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Chạy ứng dụng
app.Run();
