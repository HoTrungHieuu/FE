namespace FE
{
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;

    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var accountId = context.Session.GetString("AccountId");

            if (string.IsNullOrEmpty(accountId) && !context.Request.Path.StartsWithSegments("/Auth/Login"))
            {
                context.Response.Redirect("/Auth/Login");
                return;
            }

            await _next(context);
        }
    }

}
