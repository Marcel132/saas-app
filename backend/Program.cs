using System.IO.Compression;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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


// Console.WriteLine($"JWT KEY: {JwtKey}");
// Console.WriteLine($"JWT ISSUER: {JwtIssuer}");
// Console.WriteLine($"JWT AUDIENCE: {JwtAudience}");

builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseNpgsql(
      builder.Configuration.GetConnectionString("DefaultConnection"),
      o => o
        .MapEnum<ContractStatus>("contract_status")
        .MapEnum<ContractExecutionStatus>("contract_execution_status")
        .MapEnum<ContractApplicationStatus>("contract_application_status")  
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
        "Invalid request data",
        DomainErrorCodes.ValidationCodes.InvalidModel
      )
    );
  };
});

builder.Services.AddApiVersioning(options =>
{
  options.DefaultApiVersion = new ApiVersion(1, 0);
  options.AssumeDefaultVersionWhenUnspecified = true;
  options.ReportApiVersions = true;
});

builder.Services.AddAuthentication("CookieAuth")
    .AddScheme<AuthenticationSchemeOptions, CookieAuthHandler>(
        "CookieAuth", null);

builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddAuthorization();

builder.Services.AddApplicationService();
builder.Services.AddInfrastructureService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseMiddleware<GlobalErrorHandlingMiddleware>();
app.UseMiddleware<NetworkMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
