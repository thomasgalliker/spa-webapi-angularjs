using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using HomeCinema.Web.Infrastructure.Extensions;

namespace HomeCinema.Web.Infrastructure.MessageHandlers
{
    public class AuthenticationHandler : DelegatingHandler
    {
        IEnumerable<string> authHeaderValues = null;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                request.Headers.TryGetValues("Authorization", out this.authHeaderValues);
                if (this.authHeaderValues == null)
                {
                    return base.SendAsync(request, cancellationToken);
                }

                var tokens = this.authHeaderValues.FirstOrDefault();
                tokens = tokens?.Replace("Basic", "").Trim();
                if (!string.IsNullOrEmpty(tokens))
                {
                    byte[] data = Convert.FromBase64String(tokens);
                    string decodedString = Encoding.UTF8.GetString(data);
                    string[] tokensValues = decodedString.Split(':');
                    var username = tokensValues[0];
                    var password = tokensValues[1];
                    var membershipService = request.GetMembershipService();

                    var membershipContext = membershipService.ValidateUser(username, password);
                    if (membershipContext.User != null)
                    {
                        Thread.CurrentPrincipal = HttpContext.Current.User = membershipContext.Principal;
                        return base.SendAsync(request, cancellationToken);
                    }
                    else
                    {
                        // Authentication failed; return 401 - Unauthorized
                        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        var tsc = new TaskCompletionSource<HttpResponseMessage>();
                        tsc.SetResult(response);
                        return tsc.Task;
                    }
                }
                else
                {
                    var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    var tsc = new TaskCompletionSource<HttpResponseMessage>();
                    tsc.SetResult(response);
                    return tsc.Task;
                }
            }
            catch(Exception ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);
                return tsc.Task;
            }
        }
    }
}