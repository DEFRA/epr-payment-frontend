using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Extension;
using EPR.Payment.Portal.Middlewares;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDependencies();
builder.Services.Configure<DashboardConfiguration>(builder.Configuration.GetSection(DashboardConfiguration.SectionName));
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();
// Use the static files middleware extension
app.UseStaticFilesMiddlewareExtension();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/PaymentError/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
//app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=PaymentError}/{action=Index}/{id?}");

app.Run();
