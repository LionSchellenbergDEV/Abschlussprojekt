using Zeiterfassung.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using Zeiterfassung.Components;
using DTSAG.Common.RestClient;
using Zeiterfassung;
using Zeiterfassung.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console() 
    .WriteTo.File("logs/login-log-.txt", // <<< WICHTIG: Pfad zur Log-Datei
                  rollingInterval: RollingInterval.Day, // Täglich neue Datei
                  outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}"));
// Blazor Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddScoped<ISageClientConfig, SageClientConfig>();
builder.Services.AddScoped<SageRestClient>();
builder.Services.AddScoped<SageTicketRepository>();
builder.Services.AddScoped<TimestampRepository>();


builder.Services.AddSingleton<MailCacheService>();

// Authentication / Authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.Cookie.HttpOnly = true; // sicherer
        options.Cookie.SameSite = SameSiteMode.Lax; // für interne Navigation besser
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Middleware Reihenfolge
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication muss **vor** Authorization stehen
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Endpoint zum Setzen des Login-Cookies
app.MapGet("/set-cookie-endpoint", async (HttpContext context, string key, string value, int? days) =>
{
    var options = new CookieOptions
    {
        Expires = days.HasValue ? DateTimeOffset.UtcNow.AddDays(days.Value) : (DateTimeOffset?)null,
        IsEssential = true,
        Secure = context.Request.IsHttps,
        HttpOnly = true, // sicherer
        SameSite = SameSiteMode.Lax
    };

    context.Response.Cookies.Append(key, value, options);

    Console.WriteLine($"SERVER: Cookie '{key}' successfully set.");

    // Claims-basiertes SignIn für Authentication
    var claims = new List<System.Security.Claims.Claim>
    {
        new(System.Security.Claims.ClaimTypes.Name, key)
    };
    var identity = new System.Security.Claims.ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new System.Security.Claims.ClaimsPrincipal(identity);

    await context.SignInAsync(principal);

    return Results.Redirect("/", permanent: false);
}).WithName("SetCookieEndpoint");

app.MapGet("/abmelden", async (HttpContext context) =>
{
    //Löscht die beiden Login-Cookies
    context.Response.Cookies.Delete("UserLogin");
    context.Response.Cookies.Delete(".AspNetCore.Cookies");

    return Results.Redirect("/", permanent: false);
});

app.Run();
