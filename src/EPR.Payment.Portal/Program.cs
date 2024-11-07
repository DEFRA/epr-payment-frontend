using EPR.Payment.Portal.AppStart;
using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Extension;
using EPR.Payment.Portal.Helpers;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddFeatureManagement();
builder.Services.AddControllersWithViews().AddViewLocalization();
builder.Services.AddPortalDependencies(builder.Configuration);
builder.Services.AddServiceHealthChecks();
builder.Services.AddDependencies();
builder.Services.Configure<DashboardConfiguration>(builder.Configuration.GetSection(DashboardConfiguration.SectionName));
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddDataProtection();
builder.Services.AddLogging();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services
    .AddHttpContextAccessor()
    .RegisterWebComponents(builder.Configuration);

var app = builder.Build();
app.UseSession();
app.UseRequestLocalization();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseHealthChecks();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Error}/{action=Index}/{id?}");

await app.RunAsync();
