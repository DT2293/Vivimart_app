using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Services.Account;
using WebApplication1.Services.SystemService;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình kết nối cơ sở dữ liệu
string strcnn = builder.Configuration.GetConnectionString("cnn");
builder.Services.AddDbContext<AuthContext>(options => options.UseSqlServer(strcnn));

// Thêm các dịch vụ tùy chỉnh
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

// Cấu hình Identity và DbContext
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AuthContext>()
                .AddDefaultTokenProviders();

// Cấu hình các dịch vụ MVC
builder.Services.AddControllersWithViews();

// Cấu hình cache cho session
builder.Services.AddDistributedMemoryCache();

// Cấu hình PdfConverter với DinkToPdf
builder.Services.AddSingleton<IVnPayService, VnPayService>();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

// Cấu hình Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session sau 30 phút
    options.Cookie.HttpOnly = true; // Cookie chỉ được truy cập từ server, bảo mật cao hơn
    options.Cookie.IsEssential = true; // Cookie này cần thiết cho ứng dụng
    options.Cookie.Name = "cart"; // Đặt tên Session (sử dụng cho Cookie)
});

// Thêm HttpContextAccessor để truy cập HttpContext từ các dịch vụ
builder.Services.AddHttpContextAccessor();

// Cấu hình Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("vi-VN"), new CultureInfo("en-US") };

    options.DefaultRequestCulture = new RequestCulture("vi-VN");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Cấu hình xác thực (Authentication)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";  // Đảm bảo LoginPath đúng
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    options.SlidingExpiration = true;
                });

// Cấu hình dịch vụ MVC
var app = builder.Build();

// Cấu hình localization cho ứng dụng
app.UseRequestLocalization();

// Cấu hình bảo mật và xử lý lỗi trong môi trường sản phẩm
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Sử dụng HSTS cho sản phẩm để tăng cường bảo mật
}

app.UseHttpsRedirection();  // Chuyển hướng tất cả yêu cầu HTTP sang HTTPS
app.UseStaticFiles();       // Cấp phát các file tĩnh (CSS, JS, ảnh, v.v.)

// Cấu hình đường dẫn routing cho ứng dụng
app.UseRouting();

// Kích hoạt Session trong ứng dụng
app.UseSession();

// Kích hoạt Authentication và Authorization
app.UseAuthentication();
app.UseAuthorization();

// Cấu hình route mặc định cho các controller và action
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Chạy ứng dụng
app.Run();
