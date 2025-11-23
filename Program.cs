using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.HttpOverrides;
using SpotPlacementScoreWebsite.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure forwarded headers for proxy scenarios (Azure Front Door)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | 
                               ForwardedHeaders.XForwardedProto | 
                               ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Add authentication with Entra ID
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        builder.Configuration.GetSection("AzureAd").Bind(options);
        
        // Configure for Front Door
        options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProvider = context =>
            {
                // Use the forwarded host if available
                if (context.Request.Headers.ContainsKey("X-Forwarded-Host"))
                {
                    var forwardedHost = context.Request.Headers["X-Forwarded-Host"].ToString();
                    var forwardedProto = context.Request.Headers.ContainsKey("X-Forwarded-Proto") 
                        ? context.Request.Headers["X-Forwarded-Proto"].ToString() 
                        : "https";
                    
                    context.ProtocolMessage.RedirectUri = $"{forwardedProto}://{forwardedHost}{options.CallbackPath}";
                }
                return Task.CompletedTask;
            },
            OnRemoteFailure = context =>
            {
                context.Response.Redirect("/Home/Error");
                context.HandleResponse();
                return Task.CompletedTask;
            }
        };
    })
    .EnableTokenAcquisitionToCallDownstreamApi(new[] { "https://management.azure.com/.default" })
    .AddInMemoryTokenCaches();

// Add authorization
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

// Add controllers and views
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

// Register HttpClientFactory
builder.Services.AddHttpClient();

// Register Azure services
builder.Services.AddScoped<IAzureResourceService, AzureResourceService>();
builder.Services.AddScoped<ISpotPlacementScoreService, SpotPlacementScoreService>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Use forwarded headers middleware (must be before authentication)
app.UseForwardedHeaders();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
