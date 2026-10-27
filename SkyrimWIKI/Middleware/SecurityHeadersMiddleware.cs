namespace SkyrimWIKI.Middleware;

// Adds the standard hardening response headers to every response. The content
// policy only permits what the site actually loads: its own css, js, images,
// self-hosted fonts and the JSON api. Inline styles stay allowed because a few
// views set a background gradient with a style attribute.
public sealed class SecurityHeadersMiddleware
{
    private const string ContentSecurityPolicy =
        "default-src 'self'; " +
        "img-src 'self' data:; " +
        "font-src 'self'; " +
        "style-src 'self' 'unsafe-inline'; " +
        "script-src 'self'; " +
        "connect-src 'self'; " +
        "frame-ancestors 'none'; " +
        "base-uri 'self'; " +
        "form-action 'self'; " +
        "object-src 'none'";

    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next) => _next = next;

    public Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;
        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-Frame-Options"] = "DENY";
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
        headers["Content-Security-Policy"] = ContentSecurityPolicy;

        return _next(context);
    }
}
