using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

public class CookieAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
  private readonly TokenService _tokenService;
  public CookieAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory _logger,
    TokenService tokenService,
    UrlEncoder encoder) : base(options, _logger, encoder)
  {
    _tokenService = tokenService;
  }

  protected override Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if(!Request.Cookies.TryGetValue("AuthToken", out var authToken))
      return Task.FromResult(AuthenticateResult.NoResult());

    var principal = _tokenService.ValidateAuthToken(authToken);
    if(principal is null)
      return Task.FromResult(AuthenticateResult.Fail("Invalid Auth Token"));

      var ticket = new AuthenticationTicket(principal, Scheme.Name);
      return Task.FromResult(AuthenticateResult.Success(ticket));
  }
}