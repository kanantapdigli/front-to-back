using front_to_back.DAL;
using front_to_back.Helpers;
using front_to_back.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

#region Builder

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IFileService, FileService>();

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>();

#endregion

#region App

var app = builder.Build();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=dashboard}/{action=index}/{id?}"
    );

app.MapControllerRoute(
    name:"default",
    pattern: "{controller=home}/{action=index}/{id?}"
    );


app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(); 
app.Run();

#endregion
