using System.Net;

namespace HospitalEHR.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _log;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> log)
        { _next = next; _log = log; }

        public async Task InvokeAsync(HttpContext ctx)
        {
            try { await _next(ctx); }
            catch (Exception ex)
            {
                _log.LogError(ex, "Unhandled exception on {Method} {Path}", ctx.Request.Method, ctx.Request.Path);
                ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                if (!ctx.Response.HasStarted)
                    ctx.Response.Redirect("/Home/Error");
            }
        }
    }

    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
            => app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
