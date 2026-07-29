using ASPtestShop.Auth;
using ASPtestShop.Data;
using ASPtestShop.Services.Implementations;
using ASPtestShop.Services.Implementations.Admin;
using ASPtestShop.Services.Implementations.PaymentProviders;
using ASPtestShop.Services.Implementations.User;
using ASPtestShop.Services.Interfaces;
using ASPtestShop.Services.Interfaces.Admin;
using ASPtestShop.Services.Interfaces.User;
using ASPtestShop.Services.PaymentProviders;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Thêm namespace cho JWT Bearer Authentication
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;                // Thêm namespace cho Token Validation
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Hshop"));
});
// Cấu hình Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();
// Cấu hình JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = UserCookieAuth.Scheme;
    options.DefaultSignInScheme = UserCookieAuth.Scheme;
    options.DefaultChallengeScheme = UserCookieAuth.Scheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };

    // Kiểm tra security_stamp trong JWT có khớp với user hiện tại trong DB không
    // Nếu logout/reset password đã đổi SecurityStamp thì token cũ sẽ bị từ chối
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            var tokenSecurityStamp = context.Principal?.FindFirstValue("security_stamp");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tokenSecurityStamp))
            {
                context.Fail("Token không hợp lệ");
                return;
            }

            var userManager = context.HttpContext.RequestServices
                .GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                context.Fail("User không tồn tại");
                return;
            }

            var currentSecurityStamp = await userManager.GetSecurityStampAsync(user);

            if (tokenSecurityStamp != currentSecurityStamp)
            {
                context.Fail("Token đã hết hiệu lực");
                return;
            }
        }
    };
})
.AddCookie(UserCookieAuth.Scheme, options =>
{
    options.Cookie.Name = "HShop.User.Auth";
    options.LoginPath = "/account/login";
    options.AccessDeniedPath = "/account/access-denied";

    options.ExpireTimeSpan = TimeSpan.FromHours(6);
    options.SlidingExpiration = true;

    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
})
.AddCookie(AdminCookieAuth.Scheme, options =>
{
    options.Cookie.Name = "HShop.Admin.Auth";
    options.LoginPath = "/admin/login";
    options.AccessDeniedPath = "/admin/access-denied";

    options.ExpireTimeSpan = TimeSpan.FromHours(6);
    options.SlidingExpiration = true;

    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});
//Đăng kí services cho các lớp dịch vụ
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ICouponService, CouponService>();
//admin services
builder.Services.AddScoped<IAdminProductService, AdminProductService>();
builder.Services.AddScoped<IAdminUploadService, AdminUploadService>();
builder.Services.AddScoped<IAdminCategoryService, AdminCategoryService>();
builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();
builder.Services.AddScoped<IAdminOrderService, AdminOrderService>();
builder.Services.AddScoped<IAdminReviewService, AdminReviewService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IAdminCouponService, AdminCouponService>();
//user services
builder.Services.AddScoped<IUserAuthService, UserAuthService>();
// Payment providers
builder.Services.AddScoped<IPaymentProvider, CodPaymentProvider>();
builder.Services.AddScoped<IPaymentProvider, MomoPaymentProvider>();
builder.Services.AddScoped<IPaymentProvider, ZaloPayPaymentProvider>();
builder.Services.AddScoped<IPaymentProvider, VnPayPaymentProvider>();
builder.Services.AddScoped<IPaymentProviderFactory, PaymentProviderFactory>();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// =========================================================================
// ĐOẠN CODE TEST KẾT NỐI DATABASE (THÊM VÀO ĐÂY)

// =========================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Gọi DbContext ra để kiểm tra
        var context = services.GetRequiredService<AppDbContext>();

        if (context.Database.CanConnect())
        {
            Console.WriteLine("\n==================================================");
            Console.WriteLine("[SUCCESS] KET NOI TOI SQL SERVER (Hshop) OK!");
            Console.WriteLine("====================XIN CHÀO==================\n");

            Console.WriteLine("==================================================\n");
        }
        else
        {
            Console.WriteLine("\n==================================================");
            Console.WriteLine("[ERROR] KHONG THE KET NOI TOI DATABASE HSHOP!");
            Console.WriteLine("Vui long kiem tra lai Connection String hoac Service SQL!");
            Console.WriteLine("==================================================\n");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("\n==================================================");
        Console.WriteLine($"[FATAL] LOI KHI KET NOI DB: {ex.Message}");
        Console.WriteLine("==================================================\n");
    }
}
// =========================================================================

// =========================================================================
// SEED ROLE ADMIN / CUSTOMER
// =========================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dbContext = services.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();

    var roleManager =
        services.GetRequiredService<RoleManager<IdentityRole>>();

    var userManager =
        services.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "Admin", "Customer" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    const string adminEmail = "admin@gmail.com";
    const string adminPassword = "Admin@123";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = "Administrator"
        };

        var createResult =
            await userManager.CreateAsync(adminUser, adminPassword);

        if (!createResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                createResult.Errors.Select(e => e.Description)
            );

            throw new Exception($"Không thể tạo Admin: {errors}");
        }
    }

    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    // Seed mã giảm giá mẫu nếu chưa có
    if (!await dbContext.Coupons.AnyAsync())
    {
        dbContext.Coupons.AddRange(
            new ASPtestShop.Data.Entities.Coupon
            {
                Code = "HELLOHSHOP",
                DiscountType = "FIXED",
                DiscountValue = 50000,
                MinOrderAmount = 100000,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddYears(1),
                IsActive = true
            },
            new ASPtestShop.Data.Entities.Coupon
            {
                Code = "HSHOP10",
                DiscountType = "PERCENT",
                DiscountValue = 10,
                MinOrderAmount = 200000,
                MaxDiscountAmount = 100000,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddYears(1),
                IsActive = true
            }
        );
        await dbContext.SaveChangesAsync();
    }
}
// =========================================================================
app.Run();