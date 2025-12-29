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


Console.WriteLine($"JWT KEY: {JwtKey}");
Console.WriteLine($"JWT ISSUER: {JwtIssuer}");
Console.WriteLine($"JWT AUDIENCE: {JwtAudience}");  

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

builder.Services.AddAuthentication("CookieAuth")
    .AddScheme<AuthenticationSchemeOptions, CookieAuthHandler>(
        "CookieAuth", null);

builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();


builder.Services.AddAuthorization(options =>
{
    // -----------------
    // PROFILE
    // -----------------

    options.AddPolicy("profile.create",
        policy => policy.Requirements.Add(
            new PermissionRequirement("profile.create")
        ));
    options.AddPolicy("profile.read",
        policy => policy.Requirements.Add(
            new PermissionRequirement("profile.read")
        ));
    options.AddPolicy("profile.update",
        policy => policy.Requirements.Add(
            new PermissionRequirement("profile.update")
        ));
    options.AddPolicy("profile.delete",
        policy => policy.Requirements.Add(
            new PermissionRequirement("profile.delete")
        ));


    // -----------------
    // USERS
    // -----------------

    options.AddPolicy("users.create",
        policy => policy.Requirements.Add(
            new PermissionRequirement("users.create")
        ));
    options.AddPolicy("users.read",
        policy => policy.Requirements.Add(
            new PermissionRequirement("users.read")
        ));
    options.AddPolicy("users.update",
        policy => policy.Requirements.Add(
            new PermissionRequirement("users.update")
        ));
    options.AddPolicy("users.delete",
        policy => policy.Requirements.Add(
            new PermissionRequirement("users.delete")
        ));


    // -----------------
    // Contracts
    // -----------------
    
    options.AddPolicy("contracts.create",
        policy => policy.Requirements.Add(
            new PermissionRequirement("contracts.create")
        ));
    options.AddPolicy("contracts.read",
        policy => policy.Requirements.Add(
            new PermissionRequirement("contracts.read")
        ));
    options.AddPolicy("contracts.update",
        policy => policy.Requirements.Add(
            new PermissionRequirement("contracts.update")
        ));
    options.AddPolicy("contracts.delete",
        policy => policy.Requirements.Add(
            new PermissionRequirement("contracts.delete")
        ));
});


builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<ContractsService>();
builder.Services.AddScoped<MiddlewareService>();
builder.Services.AddScoped<AuthCookieService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RoleService>();


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
