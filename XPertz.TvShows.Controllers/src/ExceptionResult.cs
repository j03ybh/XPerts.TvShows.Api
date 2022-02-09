using System.Net;

namespace XPertz.TvShows.Controllers
{
    public sealed class ExceptionResult : Exception
    {
        public ExceptionResult(HttpStatusCode httpStatusCode, string message) : base(message)
        {
            StatusCode = httpStatusCode;
        }

        public ExceptionResult(HttpStatusCode httpStatusCode, string message, Exception innerException) : base(message, innerException)
        {
            StatusCode = httpStatusCode;
        }

        public HttpStatusCode StatusCode { get; }
    }
}