using System;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;

namespace Severino.Template.Hangfire.Infra.Jobs
{
    public class DashboardAuthFilter : IDashboardAuthorizationFilter
    {
        private readonly string _user;
        private readonly string _pass;
        
        public DashboardAuthFilter(string user, string pass)
        {
            if (string.IsNullOrWhiteSpace(user))
                throw new ArgumentNullException(nameof(user));
            
            if (string.IsNullOrWhiteSpace(pass))
                throw new ArgumentNullException(nameof(pass));
            
            _user = user;
            _pass = pass;
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            var header = httpContext.Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(header))
            {
                SetChallengeResponse(httpContext);
                return false;
            }

            var authValues = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(header);

            if (!"Basic".Equals(authValues.Scheme, StringComparison.InvariantCultureIgnoreCase))
            {
                SetChallengeResponse(httpContext);
                return false;
            }

            var parameter = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authValues.Parameter));
            var parts = parameter.Split(':');

            if (parts.Length < 2)
            {
                SetChallengeResponse(httpContext);
                return false;
            }

            var username = parts[0];
            var password = parts[1];

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                SetChallengeResponse(httpContext);
                return false;
            }

            if (username == _user && password == _pass)
            {
                return true;
            }

            SetChallengeResponse(httpContext);
            return false;
        }
        
        private void SetChallengeResponse(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 401;
            httpContext.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");
            httpContext.Response.WriteAsync("Authentication is required.");
        }
    }
}