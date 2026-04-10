using HospitalEHR.Data;
using HospitalEHR.Middleware;
using HospitalEHR.Services.Implementations;
using HospitalEHR.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/hospital-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure(3)));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
    o.IdleTimeout         = TimeSpan.FromHours(8);
    o.Cookie.HttpOnly     = true;
    o.Cookie.IsEssential  = true;
    o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    o.Cookie.SameSite     = SameSiteMode.Lax;
    o.Cookie.Name         = ".HospitalEHR.Session";
});

builder.Services.AddScoped<IPatientService,      PatientService>();
builder.Services.AddScoped<IEhrService,          EhrService>();
builder.Services.AddScoped<ILabOrderService,     LabOrderService>();
builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
builder.Services.AddScoped<ITreatmentService,    TreatmentService>();
builder.Services.AddScoped<IBillingService,      BillingService>();
builder.Services.AddScoped<IUserService,         UserService>();

builder.Services.AddControllersWithViews(o =>
    o.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute()));

builder.Services.AddAntiforgery(o =>
{
    o.Cookie.Name = "EHRX-CSRF";
    o.HeaderName  = "X-CSRF-TOKEN";
    o.Cookie.HttpOnly = true;
});

builder.Services.AddResponseCompression();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db  = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var log = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
        db.Database.Migrate();
        await DbSeeder.SeedAsync(db, log);
    }
    catch (Exception ex) { Log.Error(ex, "Database seed failed"); }
}

app.UseGlobalExceptionHandler();
if (!app.Environment.IsDevelopment()) app.UseHsts();

app.Use(async (ctx, next) =>
{
    ctx.Response.Headers.Append("X-Content-Type-Options",  "nosniff");
    ctx.Response.Headers.Append("X-Frame-Options",         "SAMEORIGIN");
    ctx.Response.Headers.Append("X-XSS-Protection",        "1; mode=block");
    ctx.Response.Headers.Append("Referrer-Policy",         "strict-origin-when-cross-origin");
    await next();
});

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();
