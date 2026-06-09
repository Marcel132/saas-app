public static class ServiceExtension
{
  public static IServiceCollection AddApplicationService(this IServiceCollection services)
  {
    services.AddScoped<AuthService>();
    services.AddScoped<UserAuthenticationService>();
    services.AddScoped<UserRegisterService>();

    services.AddScoped<MeService>();

    services.AddScoped<ContractService>();

    services.AddScoped<UserCommandService>();
    services.AddScoped<UserQueryService>();

    services.AddScoped<ApplicationService>();
    services.AddScoped<AssignmentService>();

    services.AddScoped<RefreshService>();
    services.AddScoped<AuthSessionService>();
    services.AddScoped<AuthCookieService>();
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