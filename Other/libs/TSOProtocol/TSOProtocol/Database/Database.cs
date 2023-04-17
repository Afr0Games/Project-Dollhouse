//using GonzoNet;
using Parlo;
using System.Data;
using System.Data.SQLite;

namespace TSOProtocol.Database
{
    public class SQLiteConnectionPool : IDisposable
    {
        private readonly string m_ConnectionString;
        private readonly SQLiteConnection[] m_Connections;
        private readonly object m_Lock = new object();
        private int m_Index = 0;

        public SQLiteConnectionPool(string ConnectionString, int PoolSize = 100)
        {
            m_ConnectionString = ConnectionString;
            m_Connections = new SQLiteConnection[PoolSize];
        }

        public SQLiteConnection AcquireConnection()
        {
            lock (m_Lock)
            {
                if (m_Index >= m_Connections.Length)
                    m_Index = 0;

                if (m_Connections[m_Index] == null)
                {
                    m_Connections[m_Index] = new SQLiteConnection(m_ConnectionString);
                    m_Connections[m_Index].Open();
                }

                SQLiteConnection Connection = m_Connections[m_Index];
                m_Index++;

                if (Connection.State != System.Data.ConnectionState.Open)
                    Connection.Open();

                return Connection;
            }
        }

        ~SQLiteConnectionPool()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this SQLiteConnectionPool instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this SQLiteConnectionPool instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_Connections.Length > 0)
                {
                    for (int i = 0; i < m_Connections.Length; i++)
                        m_Connections[i].Dispose();
                }

                // Prevent the finalizer from calling ~SQLiteConnectionPool, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                Logger.Log("SQLiteConnectionPool not explicitly disposed!", LogLevel.error);
        }
    }

    public class Database
    {
        private static SQLiteConnectionPool m_ConnectionPool;

        static Database()
        {
            m_ConnectionPool = new SQLiteConnectionPool(/*"Data source=:memory:"*/"Data source=Users.db");
        }

        /// <summary>
        /// Creates tables for users.
        /// </summary>
        public static void CreateTables()
        {
            SQLiteCommand Cmd = new SQLiteCommand(m_ConnectionPool.AcquireConnection());

            Cmd.CommandText = "DROP TABLE IF EXISTS Users";
            Cmd.ExecuteNonQuery();

            Cmd.CommandText = @"CREATE TABLE Users(id INTEGER PRIMARY KEY, Username TEXT, Salt TEXT, Verifier TEXT)";
            Cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Inserts the specified values into the specified columns in the specified table.
        /// </summary>
        /// <param name="Table">The table to insert into.</param>
        /// <param name="ColumnNames">The column names in the specified table.</param>
        /// <param name="Values">The values to insert.</param>
        /// <exception cref="ArgumentException">Thrown if the number of columns didn't match the number of values,
        ///                                     or any of the arguments were empty.</exception>
        public static void InsertInto(string Table, string[] ColumnNames, object[] Values)
        {
            if (Table == string.Empty || ColumnNames.Length < 1 || Values.Length < 1)
                throw new ArgumentException("Table, ColumnNames and Values cannot be empty!");
            if (ColumnNames.Length != Values.Length)
                throw new ArgumentException("ColumnNames and Values arrays must have the same length.");

            SQLiteCommand Cmd = new SQLiteCommand(m_ConnectionPool.AcquireConnection());

            Cmd.CommandText = "INSERT INTO " + Table + " (" + string.Join(", ", ColumnNames) + ") VALUES (";

            for (int i = 0; i < Values.Length; i++)
            {
                Cmd.CommandText += "@" + ColumnNames[i] + (i < Values.Length - 1 ? ", " : "");
                Cmd.Parameters.AddWithValue("@" + ColumnNames[i], Values[i]);
            }

            Cmd.CommandText += ")";
            Cmd.ExecuteNonQuery();
        }

        public static DataTable SelectFrom(string table, string conditionColumn, object conditionValue)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(m_ConnectionPool.AcquireConnection()))
            {
                cmd.CommandText = $"SELECT * FROM {table} WHERE {conditionColumn} = @ConditionValue";
                cmd.Parameters.AddWithValue("@ConditionValue", conditionValue);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    DataTable result = new DataTable();
                    result.Load(reader);
                    return result;
                }
            }
        }

        ~Database()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this Database instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this Database instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_ConnectionPool != null)
                    m_ConnectionPool.Dispose();

                // Prevent the finalizer from calling ~Database, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                Logger.Log("Database not explicitly disposed!", LogLevel.error);
        }
    }
}
