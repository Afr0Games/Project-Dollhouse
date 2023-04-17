namespace TSOProtocol
{
    /// <summary>
    /// Thrown when something network-related goes awry!
    /// </summary>
    internal class NetworkException : Exception
    {
        /// <summary>
        /// Creates a NetworkException.
        /// </summary>
        /// <param name="Message">The message that goes with the exception.</param>
        public NetworkException(string Message) : base(Message)
        {

        }
    }
}
