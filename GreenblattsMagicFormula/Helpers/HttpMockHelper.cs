using System.Net;
using System.Text;

namespace GreenblattsMagicFormula.Helpers
{
    public static class HttpMockHelper
    {
        /// <summary>
        /// Mocks the behavior of HttpMessageHandler's SendAsync method.
        /// </summary>
        /// <param name="statusCode">The HTTP status code to return.</param>
        /// <param name="responseContent">The content to include in the response.</param>
        /// <returns>A mocked HttpMessageHandler that simulates the desired HTTP response.</returns>
        public static HttpMessageHandler CreateMockHttpMessageHandler(HttpStatusCode statusCode, string responseContent)
        {
            var mockHandler = new MockHttpMessageHandler(statusCode, responseContent);
            return mockHandler;
        }

        public class MockHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _statusCode;
            private readonly string _responseContent;

            public MockHttpMessageHandler(HttpStatusCode statusCode, string responseContent)
            {
                _statusCode = statusCode;
                _responseContent = responseContent;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = _statusCode,
                    Content = new StringContent(_responseContent, Encoding.UTF8, "application/json")
                });
            }
        }
    }
}
