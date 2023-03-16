using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace TSOProtocol.Tests
{
    [TestClass]
    public class UserCacheTests
    {
        private UserCache m_Cache;

        [TestInitialize]
        public void Initialize()
        {
            m_Cache = new UserCache("TestUsers.cache", 0x12C00000, TimeSpan.FromMilliseconds(500));
        }

        [TestMethod]
        /// <summary>
        /// Tests whether AddUser correctly adds a user to the cache.
        /// </summary>
        public void AddUser_ShouldAddUserToCache()
        {
            // Arrange
            User testUser = new User { Salt = "testSalt", Username = "testUser", Verifier = "testVerifier" };

            // Act
            Database.CreateTables();
            m_Cache.AddUser(testUser);

            // Assert
            User retrievedUser = m_Cache.GetUser(testUser.Username);
            Assert.AreEqual(testUser.Username, retrievedUser.Username);
            Assert.AreEqual(testUser.Salt, retrievedUser.Salt);
            Assert.AreEqual(testUser.Verifier, retrievedUser.Verifier);
        }

        [TestMethod]
        /// <summary>
        /// Tests whether GetUser returns null when the specified user does not exist.
        /// </summary>
        public void GetUser_ShouldReturnNullWhenUserDoesNotExist()
        {
            // Act
            User retrievedUser = m_Cache.GetUser("nonexistentUser");

            // Assert
            Assert.IsNull(retrievedUser);
        }

        [TestMethod]
        /// <summary>
        /// Tests whether GetUser correctly expires a user from the cache after the specified timeout.
        /// </summary>
        public void GetUser_ShouldExpireUserAfterTimeout()
        {
            // Arrange
            User testUser = new User { Salt = "testSalt", Username = "testUser", Verifier = "testVerifier" };
            m_Cache.AddUser(testUser);

            // Act
            User retrievedUser1 = m_Cache.GetUser(testUser.Username);
            Thread.Sleep(550); // Sleep for longer than the cache timeout
            User retrievedUser2 = m_Cache.GetUserFromCache(testUser.Username);

            // Assert
            Assert.IsNull(retrievedUser2, "The user should be expired from the cache after the timeout");
        }

        [TestCleanup]
        public void Cleanup()
        {
            m_Cache.Dispose();
        }
    }
}