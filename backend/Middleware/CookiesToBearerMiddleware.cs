public class CookieToBearerMiddleware
{
    private readonly RequestDelegate _next;

    public CookieToBearerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // sprawdzamy, czy już nie ma Authorization
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            var token = context.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers.Append("Authorization", $"Bearer {token}");
            }
        }

        await _next(context);
    }
}
