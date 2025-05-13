using Microsoft.EntityFrameworkCore;
using finalProject.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<FinalProjectDbContext>();


builder.Services.AddDbContext<FinalProjectDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 42)) // <-- đúng version MySQL bạn đang dùng
    )
);
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // thời gian sống của session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();