

using backend.Application.Services;
using backend.Domain.Entities.Synchronizer;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;
using backend.Domain.Interfaces.Services;
using backend.Domain.Policies;
using backend.Infrastructure;
using backend.Infrastructure.Persistence.Repositories;
using backend.Infrastructure.Security;

public static class ServiceExtension
{
  public static IServiceCollection AddApplicationService(this IServiceCollection services)
  {
    services.AddScoped<IAuthService, AuthService>();
    services.AddScoped<IContractService, ContractService>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<ICredentialsService, CredentialsService>();

    
    services.AddScoped<UserAuthenticationService>();
    services.AddScoped<UserRegisterService>();


    services.AddScoped<ApplicationService>();
    services.AddScoped<AssignmentService>();

    services.AddScoped<AuthSessionService>();
    services.AddScoped<TokenService>();
    services.AddScoped<RoleService>();

    services.AddScoped<UserRoleSynchronizer>();

    return services;
  }

  public static IServiceCollection AddInfrastructureService(this IServiceCollection services)
  {
    services.AddScoped<ILoginPolicy, LoginPolicy>();
    services.AddScoped<IRegisterPolicy, RegisterPolicy>();

    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IUserQueryRepository, UserQueryRepository>();

    services.AddScoped<ISessionRepository, SessionRepository>();
    services.AddScoped<ISessionQueryRepository, SessionQueryRepository>();

    services.AddScoped<IRoleRepository, RoleRepository>();
    services.AddScoped<IContractRepository, ContractRepository>();
    services.AddScoped<IContractQueryRepository, ContractQueryRepository>();

    services.AddScoped<IApplicationRepository, ApplicationRepository>();
    services.AddScoped<IApplicationQueryRepository, ApplicationQueryRepository>();
    services.AddScoped<IAssignmentRepository, AssignmentRepository>();


    services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();

    return services;

  }
}