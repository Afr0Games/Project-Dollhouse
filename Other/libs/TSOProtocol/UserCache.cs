using GonzoNet;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Runtime.Caching;
using System.IO;
using System.Net.PeerToPeer;

namespace TSOProtocol
{
    /// <summary>
    /// Represents a cache for the database, used to retrieve users and cache them.
    /// </summary>
    public class UserCache : IDisposable
    {
        private MemoryCache m_Cache;
        private readonly string m_CacheFilePath;
        private readonly TimeSpan m_CacheItemSlidingExpiration;
        private readonly long m_CacheSizeLimitInBytes;

        /// <summary>
        /// Creates a new instance of UserCache.
        /// </summary>
        /// <param name="CacheFilePath">The path at which to store the cache on disk.</param>
        /// <param name="CacheSizeLimitInBytes">The size limit of the cache, in bytes.</param>
        public UserCache(string CacheFilePath, long CacheSizeLimitInBytes, TimeSpan SlidingExpiration)
        {
            m_CacheFilePath = CacheFilePath;

            // Initialize the cache with the specified memory limit and sliding expiration
            m_Cache = new MemoryCache("UserCache", new NameValueCollection
            {
                { "cacheMemoryLimitMegabytes", (CacheSizeLimitInBytes / (1024 * 1024)).ToString() }
            });

            m_CacheItemSlidingExpiration = SlidingExpiration;

            // Load cached users from file (if exists)
            LoadCacheFromFile();
        }

        /// <summary>
        /// Retrieves a user from the DB.
        /// </summary>
        /// <param name="Username">The name of the user to retrieve.</param>
        /// <returns>A new user if it was found, or am empty (not null) instance of User if not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if Username is null.</exception>
        /// <exception cref="ArgumentException">Thrown if Username is empty.</exception>
        public User GetUser(string Username)
        {
            if (Username == null)
                throw new ArgumentException("Username");
            if (Username == string.Empty)
                throw new ArgumentException("Username cannot be empty!");

            // Try to get user from cache
            User? user = m_Cache.Get(Username) as User;
            if (user != null)
                return user;

            // User not found in cache, try to get from database
            DataTable result = Database.SelectFrom("Users", "Username", Username);

            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];
                User U = new User
                {
                    Username = (string)row["Username"],
                    Salt = (string)row["Salt"],
                    Verifier = (string)row["Verifier"]
                };

                m_Cache[Username] = U;

                // Save cache to file
                SaveCacheToFile();

                return U;
            }

            // Return null when the user is not found
            return null;
        }

        /// <summary>
        /// Retrieves a user from the cache, without querying the database.
        /// </summary>
        /// <param name="Username">The name of the user.</param>
        /// <returns> A user instance.</returns>
        public User GetUserFromCache(string Username)
        {
            User? user = m_Cache.Get(Username) as User;
            return user;
        }

        /// <summary>
        /// Adds a user to the DB.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if User is null.</exception>
        public void AddUser(User User)
        {
            if (User == null)
                throw new ArgumentException("Username");

            // Check if the user already exists in the database
            DataTable result = Database.SelectFrom("Users", "Username", User.Username);

            // If the user doesn't exist, insert them into the database
            if (result.Rows.Count == 0)
            {
                Database.InsertInto("Users", new string[] { "Username", "Salt", "Verifier" },
                    new string[] { User.Username, User.Salt, User.Verifier });
            }

            CacheItemPolicy Policy = new CacheItemPolicy
            {
                SlidingExpiration = m_CacheItemSlidingExpiration
            };

            m_Cache.Add(User.Username, User, Policy);

            // Save cache to file
            SaveCacheToFile();
        }

        /// <summary>
        /// Removes a user from the DB.
        /// </summary>
        /// <param name="Username">The Username of the user to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if Username is null.</exception>
        /// <exception cref="ArgumentException">Thrown if Username is empty.</exception>
        public void RemoveUser(string Username)
        {
            if (Username == null)
                throw new ArgumentException("Username");
            if (Username == string.Empty)
                throw new ArgumentException("Username cannot be empty!");

            m_Cache.Remove(Username);

            // Save cache to file
            SaveCacheToFile();
        }

        public void InvalidateCache()
        {
            m_Cache.Dispose();
            m_Cache = new MemoryCache("UserCache", new NameValueCollection
            {
                { "cacheMemoryLimitMegabytes", (m_CacheSizeLimitInBytes / (1024 * 1024)).ToString() }
            });

            if (File.Exists(m_CacheFilePath))
                File.Delete(m_CacheFilePath);
        }

        private void SaveCacheToFile()
        {
            // Save cache to file
            using (var Writer = new BinaryWriter(File.Open(m_CacheFilePath, FileMode.Create, FileAccess.ReadWrite,
                FileShare.ReadWrite)))
            {
                Writer.Write(m_Cache.GetCount());

                foreach (var user in m_Cache)
                {
                    Writer.Write(((User)user.Value).Username);
                    Writer.Write(((User)user.Value).Salt);
                    Writer.Write(((User)user.Value).Verifier);
                }
            }
        }

        private void LoadCacheFromFile()
        {
            if (File.Exists(m_CacheFilePath))
            {
                using (var reader = new BinaryReader(File.Open(m_CacheFilePath, FileMode.Open, FileAccess.ReadWrite,
                    FileShare.ReadWrite)))
                {
                    int count = reader.ReadInt32();

                    for (int i = 0; i < count; i++)
                    {
                        var user = new User
                        {
                            Username = reader.ReadString(),
                            Salt = reader.ReadString(),
                            Verifier = reader.ReadString()
                        };

                        m_Cache.Set(user.Username, user, new CacheItemPolicy { SlidingExpiration = m_CacheItemSlidingExpiration });
                    }
                }
            }
        }


        ~UserCache()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this UserCache instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this UserCache instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual async void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_Cache != null)
                    m_Cache.Dispose();

                // Prevent the finalizer from calling ~UserCache, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                Logger.Log("UserCache not explicitly disposed!", LogLevel.error);
        }
    }
}