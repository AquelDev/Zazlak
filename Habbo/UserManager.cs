using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Zazlak.Storage;

using Zazlak;

namespace Zazlak.Habbo
{
    class UserManager
    {
        public static Hashtable _Users = new Hashtable();
        private static Thread pingChecker;
        private static int _peakUserCount;

        public static void Init()
        {
            try { pingChecker.Abort(); }
            catch { }

            try
            {
                pingChecker = new Thread(new ThreadStart(checkPings));
                pingChecker.Priority = ThreadPriority.Lowest;
                pingChecker.Start();
            }
            catch { }
        }

        public static DateTime OldStamp = new DateTime(1970, 1, 1);

        public static void addUser(int userID, User User)
        {
            if (_Users.ContainsKey(userID))
            {
                User oldUser = ((User)_Users[userID]);
                oldUser.Disconnect();
                if (_Users.ContainsKey(userID))
                    _Users.Remove(userID);
            }

            MySQL dbClient = new MySQL(Zazlak.Init.ServerMySQL);
            dbClient.Query("SELECT IP FROM users_characters WHERE username = '" + User.HabboUser.UserRow["username"] + "' LIMIT 1");
            if (User.IP == (string)dbClient.Fetch_Assoc()["ip"])
            {
                _Users.Add(userID, User);
                //dbClient.runQuery("UPDATE users SET ticket_sso = NULL WHERE id = '" + userID + "' LIMIT 1");
                //dbClient.runQuery("UPDATE users SET logins = logins + 1 WHERE id = '" + userID + "'");
                dbClient.Query("UPDATE users_characters SET lastaccess = '" + (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds + "' WHERE id = '1'");
                dbClient.Query("UPDATE users_characters SET lastaccess = '" + (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds + "' WHERE id = '2'");
                dbClient.Query("UPDATE users_characters SET connected = 1 WHERE id = '" + userID + "'");
                Out.WriteLine(User.HabboUser.UserRow["username"] + " has connected", ConsoleColor.DarkMagenta);
            }

        }

        #region Users
        public static void removeUser(int userID)
        {
            if (_Users.ContainsKey(userID))
            {
                _Users.Remove(userID);
            }
        }

        public static bool containsUser(int userID)
        {
            return _Users.ContainsKey(userID);
        }

        public static bool containsUser(string userName)
        {
            int userID = getUserID(userName);
            return _Users.ContainsKey(userID);
        }

        public static int userCount
        {
            get
            {
                return _Users.Count;
            }
        }

        public static int peakUserCount
        {
            get
            {
                return _peakUserCount;
            }
        }

        public static int getUserID(string userName)
        {
            MySQL dbClient = new MySQL(Zazlak.Init.ServerMySQL);
            dbClient.Query("SELECT id FROM users WHERE name = " + userName + "' LIMIT 1");
            return Convert.ToInt32(dbClient.Fetch_Assoc()["id"]);
        }

        public static string getUserName(int userID)
        {
            MySQL dbClient = new MySQL(Zazlak.Init.ServerMySQL);
            dbClient.Query("SELECT name FROM users WHERE id = '" + userID + "' LIMIT 1");
            return Convert.ToString(dbClient.Fetch_Assoc()["username"]);
        }

        public static bool userExists(int userID)
        {
            MySQL dbClient = new MySQL(Zazlak.Init.ServerMySQL);
            dbClient.Query("SELECT id FROM users WHERE id = '" + userID + "'");
            return Convert.ToBoolean(dbClient.Num_Rows());
        }

        #endregion
        private static void checkPings()
        {
            List<User> vir = new List<User>();
            while (true)
            {
                lock (_Users)
                {
                    try
                    {
                        foreach (User User in _Users.Values)
                        {
                                if (User.pingOK)
                                {
                                    User.pingOK = false;
                                }
                                else
                                {
                                    vir.Add(User);
                                }
                        }
                    }
                    catch { }
                    foreach (User User in vir)
                    {
                        User.Disconnect();
                    }
                }
                vir.Clear();

                Thread.Sleep(100000);
            }
        }
    }
}
