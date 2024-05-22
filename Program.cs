using dotnetstartermvc.Data;
using dotnetstartermvc.ExtendMethods;
using dotnetstartermvc.Menu;
using dotnetstartermvc.Models;
using dotnetstartermvc.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AppMvcConnectionString");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    // {0} -> ten Action
    // {1} -> ten Controller
    // {2} -> ten Area
    options.ViewLocationFormats.Add("/MyView/{1}/{0}" + RazorViewEngine.ViewExtension);

    options.AreaViewLocationFormats.Add("/MyAreas/{2}/Views/{1}/{0}.cshtml");
});

builder.Services.AddDistributedMemoryCache();           // Đăng ký dịch vụ lưu cache trong bộ nhớ (Session sẽ sử dụng nó)
builder.Services.AddSession(cfg =>
{                    // Đăng ký dịch vụ Session
    cfg.Cookie.Name = "appmvc";                 // Đặt tên Session - tên này sử dụng ở Browser (Cookie)
    cfg.IdleTimeout = new TimeSpan(0, 30, 0);    // Thời gian tồn tại của Session
});

// dang ky dich vu email
builder.Services.AddOptions();
var mailsetting = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailsetting);
builder.Services.AddSingleton<IEmailSender, SendMailService>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSession();

// Dang ky Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2); // Khóa 2 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lần thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất


    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login/";
    options.LogoutPath = "/logout/";
    options.AccessDeniedPath = "/khongduoctruycap.html";
});

builder.Services.AddAuthentication()
                    .AddGoogle(options =>
                    {
                        var gconfig = builder.Configuration.GetSection("Authentication:Google");
                        options.ClientId = gconfig["ClientId"];
                        options.ClientSecret = gconfig["ClientSecret"];
                        options.CallbackPath = "/dang-nhap-tu-google";
                    })
                    .AddFacebook(options =>
                    {
                        var fconfig = builder.Configuration.GetSection("Authentication:Facebook");
                        options.AppId = fconfig["AppId"];
                        options.AppSecret = fconfig["AppSecret"];
                        options.CallbackPath = "/dang-nhap-tu-facebook";
                    })
                    // .AddTwitter()
                    // .AddMicrosoftAccount()
                    ;

builder.Services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ViewManageMenu", builder =>
    {
        builder.RequireAuthenticatedUser();
        builder.RequireRole(RoleName.SuperAdmin, RoleName.Administrator, RoleName.Manager, RoleName.Member);
    });
});

builder.Services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddTransient<AdminSidebarService>();

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
// /contents/1.jpg => Uploads/1.jpg
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Uploads")
    ),
    RequestPath = "/contents"
});

app.UseSession();
app.AddStatusCodePage(); // Tuy bien Response loi: 400 - 599

app.UseRouting();

app.UseAuthentication(); // xac dinh danh tinh 
app.UseAuthorization();  // xac thuc  quyen truy  cap

app.MapControllers();
app.MapControllerRoute(
                name: "first",
                pattern: "{url:regex(^((xemsanpham)|(viewproduct))$)}/{id:range(2,4)}",
                defaults: new
                {
                    controller = "First",
                    action = "ViewProduct"
                }

            );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Home}/{id?}");

app.MapAreaControllerRoute(
                name: "product",
                pattern: "/{controller}/{action=Index}/{id?}",
                areaName: "ProductManage"
            );

app.MapRazorPages();

app.Run();