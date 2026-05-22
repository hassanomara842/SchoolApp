using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Models;
using SchoolApp.Hubs;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using QuestPDF.Infrastructure;
using SchoolApp.Services;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddHttpClient<IAiQuizService, GeminiQuizService>();
builder.Services.AddHttpClient<IPaymobService, PaymobService>();

builder.Services.AddSignalR();

builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? throw new InvalidOperationException("Google ClientId not found.");
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret not found.");
    });
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

var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("ar-EG")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<ChatHub>("/chatHub");

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    
    string[] roleNames = { "Admin", "Instructor", "Trainee" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
    
    var adminUser = await userManager.FindByEmailAsync("admin@school.com");
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = "admin@school.com",
            Email = "admin@school.com",
            FullName = "Administrator"
        };
        await userManager.CreateAsync(adminUser, "Admin@123");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!dbContext.Announcements.Any())
    {
        dbContext.Announcements.Add(new Announcement
        {
            Title = "Welcome to the New Term!",
            Description = "We are excited to start this new learning journey with you. Explore our newest courses and reach out if you have any questions.",
            ImageUrl = "https://images.unsplash.com/photo-1540317580384-e5d43616b9aa?q=80&w=600&auto=format&fit=crop",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();
    }
}

app.Run();
