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

    services.AddScoped<MeService>();



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

    services.AddScoped<IMeQueryRepository, MeQueryRepository>();
    services.AddScoped<IMeRepository, MeRepository>();

    services.AddScoped<ISessionRepository, SessionRepository>();
    services.AddScoped<ISessionQueryRepository, SessionQueryRepository>();

    services.AddScoped<IRoleRepository, RoleRepository>();
    services.AddScoped<IContractRepository, ContractRepository>();
    services.AddScoped<IContractQueryRepository, ContractQueryRepository>();

    services.AddScoped<IApplicationRepository, ApplicationRepository>();
    services.AddScoped<IAssignmentRepository, AssignmentRepository>();


    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

    return services;

  }
}