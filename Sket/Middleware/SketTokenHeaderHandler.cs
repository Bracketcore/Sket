using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Headers;

namespace Bracketcore.Sket.Middleware
{
    public class SketTokenHeaderHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains("Authorization"))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}