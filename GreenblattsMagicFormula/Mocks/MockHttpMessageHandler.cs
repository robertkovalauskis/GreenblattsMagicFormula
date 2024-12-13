using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenblattsMagicFormula.Mocks
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _send;

        public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> send)
        {
            _send = send ?? throw new ArgumentNullException(nameof(send));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_send(request));
        }
    }
}
