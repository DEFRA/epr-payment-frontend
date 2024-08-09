using EPR.Payment.Portal.AppStart;
using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Extension;
using EPR.Payment.Portal.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddPortalDependencies(builder.Configuration);
builder.Services.AddServiceHealthChecks();
builder.Services.AddDependencies();
builder.Services.Configure<DashboardConfiguration>(builder.Configuration.GetSection(DashboardConfiguration.SectionName));
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddDataProtection();
builder.Services.AddLogging();

var app = builder.Build();

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
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Error}/{action=Index}/{id?}");

app.Run();
