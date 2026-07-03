using System.Text.Json.Serialization;
using backend.Api.Auth;
using backend.Api.Controllers.Auth.DTOs;
using backend.Api.Http;
using backend.Api.Middlewares;
using backend.Domain.Entities.Enum;
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
builder.Services.AddCors(options =>
{
  options.AddPolicy("AngularPolicy", policy =>
  {
    policy
      .WithOrigins("http://localhost:4200")
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials();
  });
});


builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseNpgsql(
      builder.Configuration.GetConnectionString("DefaultConnection"),
      o => o
        .MapEnum<ContractStatus>("contract_status")
        .MapEnum<ContractReportStatus>("report_status")
        .MapEnum<ContractApplicationStatus>("application_status")  
        .MapEnum<RoleType>("role_type")
        .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
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
        "Niepoprawne dane",
        DomainErrorCodes.ValidationCodes.InvalidModel
      )
    );
  };
});

builder.Services.Configure<AuthCookiesOptions>(
    builder.Configuration.GetSection("AuthCookies"));
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));


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
app.UseCors("AngularPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
