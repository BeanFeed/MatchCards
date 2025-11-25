using MatchCards.Extensions;
using MatchCards.Hubs;
using MatchCards.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRouting(x => x.LowercaseUrls = true);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(x =>
    {
        x.WithOrigins("http://localhost:3000", "https://matchcards.beanfeed.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromDays(365*100);
    options.SlidingExpiration = true;

    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();

builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<GameService>();



var app = builder.Build();

app.InitializeDb();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseSwagger(options =>
{
    options.RouteTemplate = "openapi/{documentName}/openapi.json";
});
app.MapScalarApiReference(options =>
{
    options.OpenApiRoutePattern = "openapi/{documentName}/openapi.json";
});

app.UseCors();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.UseSpaStaticFiles();
if (!app.Environment.IsDevelopment())
{
    app.UseSpa(spa =>
    {
        spa.Options.SourcePath = "wwwroot/";
    });
}

app.MapHub<GameHub>("/hubs/game");

app.Run();