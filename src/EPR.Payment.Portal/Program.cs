using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.Options;
using EPR.Payment.Portal.Extension;
using EPR.Payment.Portal.HealthCheck;
using EPR.Payment.Portal.Helpers;
using EPR.Payment.Portal.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.FeatureManagement;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var builderConfig = builder.Configuration;

// Validate and retrieve GlobalVariables
var globalVariables = builderConfig.Get<GlobalVariables>()
                     ?? throw new InvalidOperationException("GlobalVariables configuration is missing.");

string basePath = globalVariables.BasePath
                 ?? throw new InvalidOperationException("BasePath is not configured in GlobalVariables.");

// Add services to the container
builder.Services.AddFeatureManagement();
builder.Services.AddControllersWithViews().AddViewLocalization();
builder.Services.AddPortalDependencies(builder.Configuration);
builder.Services.AddDependencies();
builder.Services.Configure<DashboardConfiguration>(builder.Configuration.GetSection(DashboardConfiguration.SectionName));
builder.Services.AddApplicationInsightsTelemetry().AddHealthChecks();
builder.Services.AddDataProtection();

// Configure forwarded headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    var forwardedHeadersOptions = builderConfig.GetSection("ForwardedHeaders").Get<ForwardedHeadersOptions>()
                                  ?? throw new InvalidOperationException("ForwardedHeaders configuration is missing.");

    options.ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto;
    options.ForwardedHostHeaderName = forwardedHeadersOptions.ForwardedHostHeaderName
                                      ?? throw new InvalidOperationException("ForwardedHostHeaderName is not configured in ForwardedHeaders.");
    options.OriginalHostHeaderName = forwardedHeadersOptions.OriginalHostHeaderName
                                     ?? throw new InvalidOperationException("OriginalHostHeaderName is not configured in ForwardedHeaders.");
    options.AllowedHosts = forwardedHeadersOptions.AllowedHosts
                           ?? throw new InvalidOperationException("AllowedHosts is not configured in ForwardedHeaders.");
});

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
app.UsePathBase(basePath);

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseForwardedHeaders(); // Add forwarded headers middleware

app.UseMiddleware<SecurityHeaderMiddleware>();
app.UseCookiePolicy();
app.UseMiddleware<AnalyticsCookieMiddleware>();
app.UseStaticFiles();
app.UseRouting();
app.UseHttpsRedirection();

// Check if authentication is enabled using a feature flag
var featureManager = app.Services.GetRequiredService<IFeatureManager>();
if (await featureManager.IsEnabledAsync("EnableAuthenticationFeature"))
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Error}/{action=Index}/{id?}");

app.MapHealthChecks("/admin/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new { key = e.Key, value = e.Value.Status.ToString() }),
            duration = report.TotalDuration.TotalMilliseconds
        });

        await context.Response.WriteAsync(result);
    }
}).AllowAnonymous();
await app.RunAsync();