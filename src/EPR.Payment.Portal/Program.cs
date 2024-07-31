using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProvider;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDependencies();
builder.Services.Configure<DashboardConfiguration>(builder.Configuration.GetSection(DashboardConfiguration.SectionName));
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/PaymentError/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions {
     FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "Asset")),
     RequestPath = "/Asset"
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=PaymentError}/{action=Index}/{id?}");

app.Run();
