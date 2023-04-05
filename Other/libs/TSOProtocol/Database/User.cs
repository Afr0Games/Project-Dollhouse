namespace TSOProtocol.Database
{
    /// <summary>
    /// Represents a user in the DB with the following columns needed for SRP authentication.
    /// </summary>
    public class User
    {
        public string Username = string.Empty;
        public string Salt = string.Empty;
        public string Verifier = string.Empty;
    }
}
