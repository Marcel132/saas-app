using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi("Backend");
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var JwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured in appsettings.json");
var JwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured in appsettings.json");
var JwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured in appsettings.json");


Console.WriteLine($"JWT KEY: {JwtKey}");
Console.WriteLine($"JWT ISSUER: {JwtIssuer}");
Console.WriteLine($"JWT AUDIENCE: {JwtAudience}");

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = JwtIssuer,
            ValidAudience = JwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(JwtKey)
            )
        };
    }
);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        return new BadRequestObjectResult(
            HttpResponseFactory.CreateFailureResponse<object>(
                context.HttpContext,
                HttpResponseState.BadRequest,
                false,
                "Invalid request data",
                HttpStatusCodes.ValidationCodes.InvalidModel
            )
        );
    };
});


builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<ContractsService>();
builder.Services.AddScoped<MiddlewareService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<GlobalErrorHandlingMiddleware>();
app.UseMiddleware<NetworkMiddleware>();

app.UseHttpsRedirection();

app.UseMiddleware<AuthenticationMiddleware>();
app.UseAuthentication();

app.UseMiddleware<AuthorizationMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.Run();
