namespace SwedesRankTracker.Exceptions
{
    public sealed class NotQualifiedMemberException : Exception
    {
        public NotQualifiedMemberException()
        {
        }

        public NotQualifiedMemberException(string? message) : base(message)
        {
        }

        public NotQualifiedMemberException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}