using System.Net.Http;

namespace SwedesRankTracker.Exceptions
{
    // Custom exception for explicit handling of exhausted 429 retries
    public sealed class TooManyRequestsException : HttpRequestException
    {
        public TooManyRequestsException()
        {
        }

        public TooManyRequestsException(string? message)
            : base(message)
        {
        }

        public TooManyRequestsException(string? message, Exception? inner)
            : base(message, inner)
        {
        }
    }
}