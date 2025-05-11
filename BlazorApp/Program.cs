using BlazorApp;
using BlazorApp.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

/*******************************************************************************/
// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(cookie =>
{
    //Sets the cookie name and manage, so the cookie is invalidated.  
    cookie.Cookie.Name = "keycloak.cookie";
    cookie.Cookie.MaxAge = TimeSpan.FromMinutes(60);
    cookie.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    cookie.SlidingExpiration = true;
})
.AddOpenIdConnect(options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    options.Authority = builder.Configuration.GetValue<string>("Keycloak:RealmUrl");
    options.ClientId = builder.Configuration.GetValue<string>("Keycloak:ClientId");
    options.ClientSecret = builder.Configuration.GetValue<string>("Keycloak:ClientSecret");
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.ResponseType = OpenIdConnectResponseType.Code;
    // Disable HTTPS requirement for metadata in development
    options.RequireHttpsMetadata = false;
    options.CorrelationCookie.SameSite = SameSiteMode.Lax;
    options.NonceCookie.SameSite = SameSiteMode.Lax;
    //Save the token  
    options.SaveTokens = true;
    options.Scope.Add("openid");
    options.Scope.Add("email");
    options.Scope.Add("phone");
    options.Scope.Add("profile");
    // options.NonceCookie.SameSite = SameSiteMode.None;
    // options.CorrelationCookie.SameSite = SameSiteMode.None;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "https://schemas.scopic.com/roles"
    };
});

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

/*--------------------------------------------------------------------------------------*/

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

/*******************************************/
builder.Services.AddHttpContextAccessor();
/*-----------------------------------------*/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

/******************************************/
app.MapDefaultEndpoints();
app.MapGroup("/authentication")
    .MapLoginAndLogout()
    .RequireAuthorization();
/*-----------------------------------------*/

app.Run();
