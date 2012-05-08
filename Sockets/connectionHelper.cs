using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace Zazlak.Sockets
{
    class connectionHelper
    {
        //private static int BannedIPcount;
        private static Hashtable BannedIPs = new Hashtable();
        public static Hashtable Connections = new Hashtable();
        private static Thread Checker = new Thread(new ThreadStart(CheckSockets));

        internal static void Init()
        {
            Checker.Start();
        }

        public static bool IpIsBanned(string IP)
        {
            return false;
        }

        private static void CheckSockets()
        {
            while (true)
            {
                Hashtable TMP = Connections;
                foreach (gameConnection EndPoint in TMP.Values)
                {
                    if (!EndPoint.IsActive)
                        EndPoint.Close();
                }

                Thread.Sleep(10000); // 10 secconds
            }

        }

        internal static void AddConnection(gameConnection Endpoint, int ConnectionID)
        {
            if (Connections.ContainsValue(ConnectionID))
                Connections.Remove(ConnectionID);
            Connections.Add(ConnectionID, Endpoint);
        }

        internal static void RemoveConnection(int ID)
        {
            if (Connections.ContainsValue(ID))
                Connections.Remove(ID);
        }

        internal static void BanIP(string IP)
        {
        }
    }
}
