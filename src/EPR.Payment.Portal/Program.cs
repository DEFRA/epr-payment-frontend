using EPR.Payment.Portal.AppStart;
using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.Options;
using EPR.Payment.Portal.Extension;
using EPR.Payment.Portal.Helpers;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);
var builderConfig = builder.Configuration;
var globalVariables = builderConfig.Get<GlobalVariables>();
string basePath = globalVariables.BasePath;

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

// Configure forwarded headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    var forwardedHeadersOptions = builderConfig.GetSection("ForwardedHeaders").Get<ForwardedHeadersOptions>();

    options.ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto;
    options.ForwardedHostHeaderName = forwardedHeadersOptions.ForwardedHostHeaderName;
    options.OriginalHostHeaderName = forwardedHeadersOptions.OriginalHostHeaderName;
    options.AllowedHosts = forwardedHeadersOptions.AllowedHosts;
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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseForwardedHeaders(); // Add forwarded headers middleware

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseHealthChecks();

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

await app.RunAsync();
