using System.Text;
using Microsoft.AspNetCore.Http.Extensions;

namespace SkyrimWIKI.Middleware;

// Restores ASP.NET Core Identity field names before model binding.
// The client renames form fields to random tokens; this middleware rewrites
// them back so the existing Razor Pages models keep working unchanged.
public class FormFieldObfuscationMiddleware
{
    private readonly RequestDelegate _next;

    // Map of obfuscated -> real names used by Identity/Razor Pages
    private static readonly Dictionary<string, string> ReverseMap = new(StringComparer.Ordinal)
    {
        ["_em"] = "Input.Email",
        ["_pw"] = "Input.Password",
        ["_cp"] = "Input.ConfirmPassword",
        ["_rm"] = "Input.RememberMe",
        ["_tok"] = "__RequestVerificationToken",
        ["_ret"] = "ReturnUrl",
    };

    public FormFieldObfuscationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (HttpMethods.IsPost(context.Request.Method)
            && context.Request.HasFormContentType)
        {
            // Read the form into memory so we can rewrite it
            context.Request.EnableBuffering();
            var form = await context.Request.ReadFormAsync();

            if (form.Count > 0 && form.Any(x => ReverseMap.ContainsKey(x.Key)))
            {
                var collection = new FormCollection(
                    form.ToDictionary(
                        kv => ReverseMap.TryGetValue(kv.Key, out var real) ? real : kv.Key,
                        kv => kv.Value
                    ),
                    form.Files
                );

                context.Request.Form = collection;
            }
        }

        await _next(context);
    }
}

public static class FormFieldObfuscationMiddlewareExtensions
{
    public static IApplicationBuilder UseFormFieldObfuscation(this IApplicationBuilder builder)
        => builder.UseMiddleware<FormFieldObfuscationMiddleware>();
}
