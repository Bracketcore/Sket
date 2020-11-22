using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace UnoRoute.Sket.Core.Middleware
{
    public class SketTokenHeaderHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains("Authorization")) return new HttpResponseMessage(HttpStatusCode.BadRequest);

            return await base.SendAsync(request, cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }
    }
}