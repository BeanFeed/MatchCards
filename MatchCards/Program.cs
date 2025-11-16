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



var app = builder.Build();

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

app.UseRouting();

app.Run();