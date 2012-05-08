using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Text;

namespace Zazlak.Storage
{
    /// <summary>
    /// DatabaseManager acts as a proxy towards an encapsulated Database at a DatabaseServer.
    /// </summary>
    internal class DatabaseManager
    {
        #region Fields
        private DatabaseServer mServer;
        private Database mDatabase;

        private DatabaseClient[] mClients = new DatabaseClient[0];
        private bool[] mClientAvailable = new bool[0];
        private int mClientStarvationCounter;
        private object mLockObject;

        private Task mClientMonitor;
        #endregion

        #region Properties

        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a DatabaseManager for a given DatabaseServer and Database.
        /// </summary>
        /// <param name="pServer">The DatabaseServer for this database proxy.</param>
        /// <param name="pDatabase">The Database for this database proxy.</param>
        internal DatabaseManager(DatabaseServer pServer, Database pDatabase)
        {
            mServer = pServer;
            mDatabase = pDatabase;
            mLockObject = new object();
        }
        /// <summary>
        /// Constructs a DatabaseManager for given database server and database details.
        /// </summary>
        /// <param name="sServer">The network host of the database server, eg 'localhost' or '127.0.0.1'.</param>
        /// <param name="Port">The network port of the database server as an unsigned 32 bit integer.</param>
        /// <param name="sUser">The username to use when connecting to the database.</param>
        /// <param name="sPassword">The password to use in combination with the username when connecting to the database.</param>
        /// <param name="sDatabase">The name of the database to connect to.</param>
        /// <param name="minPoolSize">The minimum connection pool size for the database.</param>
        /// <param name="maxPoolSize">The maximum connection pool size for the database.</param>
        internal DatabaseManager(string sServer, uint Port, string sUser, string sPassword, string sDatabase, uint minPoolSize, uint maxPoolSize)
        {
            mServer = new DatabaseServer(sServer, Port, sUser, sPassword);
            mDatabase = new Database(sDatabase, minPoolSize, maxPoolSize);

            mClientMonitor = new Task(MonitorClientsLoop);
            //mClientMonitor.Priority = ThreadPriority.Lowest;

            mClientMonitor.Start();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Starts the client monitor thread. The client monitor disconnects inactive clients etc.
        /// </summary>
        //internal void StartMonitor()
        //{
        //    mClientMonitor = new Task(MonitorClientsLoop);
        //    //mClientMonitor.Priority = ThreadPriority.Lowest;

        //    mClientMonitor.Start();
        //}
        /// <summary>
        /// Stops the client monitor thread.
        /// </summary>
        internal void StopMonitor()
        {
            if (mClientMonitor != null)
            {
                mClientMonitor.Dispose();
            }
        }

        /// <summary>
        /// Disconnects and destroys all database clients.
        /// </summary>
        public void DestroyClients()
        {
            lock (this.mClients)
            {
                for (int i = 0; i < mClients.Length; i++)
                {
                    // Clients[i].Destroy();
                    // Clients[i] = null;
                }
            }
        }
        /// <summary>
        /// Nulls all instance fields of the database manager.
        /// </summary>
        internal void DestroyManager()
        {
            //mServer = null;
            //mDatabase = null;
            //mClients = null;
            //mClientAvailable = null;

            //mClientMonitor = null;
        }

        /// <summary>
        /// Closes the connections of database clients that have been inactive for too long. Connections can be opened again when needed.
        /// </summary>
        private void MonitorClientsLoop()
        {
            while (true)
            {
                try
                {
                    lock (mLockObject)
                    {
                        DateTime dtNow = DateTime.Now;
                        for (int i = 0; i < mClients.Length; i++)
                        {
                            if (mClients[i].State != ConnectionState.Closed)
                            {
                                if (mClients[i].Inactivity >= 60) // Not used in the last %x% seconds
                                {
                                    mClients[i].Disconnect(); // Temporarily close connection

                                    Console.WriteLine("Disconnected database client #" + mClients[i].Handle);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                //Thread.Sleep(10000); // 10 seconds
            }
        }
        /// <summary>
        /// Creates the connection string for this database proxy.
        /// </summary>
        internal string CreateConnectionString()
        {
            MySqlConnectionStringBuilder pCSB = new MySqlConnectionStringBuilder();

            // Server
            pCSB.Server = mServer.Host;
            pCSB.Port = mServer.Port;
            pCSB.UserID = mServer.User;
            pCSB.Password = mServer.Password;

            // Database
            pCSB.Database = mDatabase.Name;
            pCSB.MinimumPoolSize = mDatabase.minPoolSize;
            pCSB.MaximumPoolSize = mDatabase.maxPoolSize;

            return pCSB.ToString();
        }

        internal DatabaseClient GetClient()
        {
            //lock (mLockObject)
            {
                for (uint i = 0; i < mClients.Length; i++)
                {
                    if (mClientAvailable[i] == true)
                    {
                        mClientAvailable[i] = false;
                        mClientStarvationCounter = 0;

                        if (mClients[i].State == ConnectionState.Closed)
                        {
                            try
                            {
                                mClients[i].Connect();
                            }
                            catch
                            {
                                mClients[i].Destroy();
                                mClients[i] = new DatabaseClient(i, this);
                                mClients[i].Connect();
                            }
                            //Logging.WriteLine("Opening connection for database client #" + mClients[i].Handle);
                        }

                        if (mClients[i].State == ConnectionState.Open)
                        {
                            mClients[i].UpdateLastActivity();
                            if (!mClients[i].IsBussy)
                            {
                                mClients[i].IsBussy = true;
                                return mClients[i];
                            }
                        }
                    }
                }

                mClientStarvationCounter++;

                if (mClientStarvationCounter >= ((mClients.Length + 1) / 2))
                {
                    mClientStarvationCounter = 0;
                    SetClientAmount((uint)(mClients.Length + 1 * 1.3f));
                    
                    return GetClient();
                }

                DatabaseClient pAnonymous = new DatabaseClient(0, this);
                pAnonymous.Connect();
                return pAnonymous;
            }
        }
        internal void ReleaseClient(uint Handle)
        {
            if (mClients.Length >= (Handle - 1)) // Ensure client exists
            {
                mClientAvailable[Handle - 1] = true;
                //Logging.WriteLine("Released client #" + Handle);
            }
        }

        /// <summary>
        /// Sets the amount of clients that will be available to requesting methods. If the new amount is lower than the current amount, the 'excluded' connections are destroyed. If the new connection amount is higher than the current amount, new clients are prepared. Already existing clients and their state will be maintained.
        /// </summary>
        /// <param name="Amount">The new amount of clients.</param>
        internal void SetClientAmount(uint Amount)
        {
            lock (this)
            {
                if (mClients.Length == Amount)
                    return;

                if (Amount < mClients.Length) // Client amount shrinks, dispose clients that will die
                {
                    for (uint i = Amount; i < mClients.Length; i++)
                    {
                        mClients[i].Destroy();
                        mClients[i] = null;
                    }
                }

                DatabaseClient[] pClients = new DatabaseClient[Amount];
                bool[] pClientAvailable = new bool[Amount];
                for (uint i = 0; i < Amount; i++)
                {
                    if (i < mClients.Length) // Keep the existing client and it's available state
                    {
                        pClients[i] = mClients[i];
                        pClientAvailable[i] = mClientAvailable[i];
                    }
                    else // We are in need of more clients, so make another one
                    {
                        pClients[i] = new DatabaseClient((i + 1), this);
                        pClientAvailable[i] = true; // Elegant?
                    }
                }

                // Update the instance fields
                mClients = pClients;
                mClientAvailable = pClientAvailable;
            }
        }

        //internal bool INSERT(IDataObject obj)
        //{
        //    using (DatabaseClient MySQL = GetClient())
        //    {
        //        return obj.INSERT(MySQL);
        //    }
        //}

        //internal bool DELETE(IDataObject obj)
        //{
        //    using (DatabaseClient MySQL = GetClient())
        //    {
        //        return obj.DELETE(MySQL);
        //    }
        //}

        //internal bool UPDATE(IDataObject obj)
        //{
        //    using (DatabaseClient MySQL = GetClient())
        //    {
        //        return obj.UPDATE(MySQL);
        //    }
        //}

        public override string ToString()
        {
            return mServer.ToString() + ":" + mDatabase.Name;
        }

        internal int ConnectionCount
        {
            get
            {
                int Count = 0;
                for (int i = 0; i < mClients.Length; i++)
                {
                    DatabaseClient Client = mClients[i];
                    if (Client == null)
                        continue;
                    if (Client.State != ConnectionState.Closed)
                        Count++;
                }

                return Count;
            }
        }
        #endregion
    }
}