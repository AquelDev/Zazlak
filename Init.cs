using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zazlak;
using Zazlak.Kernel;
using Zazlak.Storage;
using Zazlak.Sockets;
using System.Threading;
using Zazlak.Habbo;
using System.Data;

namespace Zazlak
{
    static class Init
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        internal static DateTime ServerStarted;
        internal static Encoding DefaultEncoding;
        internal static ConfigurationData Config;
        internal static DatabaseManager ServerMySQL;

        [STAThread]
        static void Main()
        {
            ServerStarted = DateTime.Now;
            DefaultEncoding = Encoding.Default;
            Console.Title = "Zazlak - now loading.";

            Out.WritePlain(@"
    _______             _        _           |
   (_______)           | |      | |          |
      __    _____ _____| | _____| |  _       | Copyright: Aquel
     / /   (____ (___  ) |(____ | |_/ )      | *-05-2012 - *-12-2013
    / /____/ ___ |/ __/| |/ ___ |  _ (       | RELEASE63-201205022303-217664704
   (_______)_____(_____)\_)_____|_| \_)      |", ConsoleColor.White);
            Out.WritePlain("                                             |", ConsoleColor.White);
            Out.WritePlain("  ----------------------------------------------------------------------------", ConsoleColor.White);
            Out.WritePlain("");
            

            

            #region GetConfig
            String Host = "";
            UInt32 Port = 0;
            String User = "";
            String Pass = "";
            String DbName = "";

            Int32 GamePort = 0;
            Int32 ConnectionsLimits = 0;

            try
            {
                Config = new ConfigurationData("config.conf");
                Host = Config.data["db.host"];
                Port = Convert.ToUInt32(Config.data["db.port"]);
                User = Config.data["db.username"];
                Pass = Config.data["db.password"];
                DbName = Config.data["db.name"];

                GamePort = Convert.ToInt32(Config.data["game.tcp.port"]);
                ConnectionsLimits = Convert.ToInt32(Config.data["game.tcp.limitusers"]);
            }
            catch (Exception Error)
            {
                Out.WritePlain("[Zazlak] " + Error.Message, ConsoleColor.DarkRed);
                Console.ReadKey();
                Environment.Exit(0);
            }

            #endregion
            #region CheckConfig
            if (Host == "" || Port.ToString() == "" || User == "" || Pass == "" || DbName == "" || Pass == "root" || GamePort.ToString() == "" || ConnectionsLimits.ToString() == "")
            {
                Out.WritePlain("[Zazlak] > Do not leave blank fields in the Config.", ConsoleColor.DarkRed);
                Console.ReadKey();
            }
            else
            {
                Out.WritePlain("[Zazlak] > Configurations loaded!", ConsoleColor.Green);
            }
            #endregion
            #region LoadMySQL
            try
            {
                DatabaseServer dbServer = new DatabaseServer(Host, Port, User, Pass);
                Database db = new Database(DbName, 5, 5000);

                ServerMySQL = new DatabaseManager(dbServer, db);

                /*
                MySQL Lol = new MySQL(ServerMySQL);
                Lol.Query("SELECT username FROM users_characters");
                Out.WriteLine(Lol.Array()["username"]);
                 */
            }
            catch (Exception Error)
            {
                Out.WritePlain("[MYSQL] > " + Error.Message, ConsoleColor.DarkRed);
                Console.ReadKey();
            }
            Out.WritePlain("[Zazlak] > MySQL Connected.", ConsoleColor.Green);
            #endregion
            #region LoadHabbo
            Habbo.Cache.Catalog.InitCatalogPages();
            Habbo.Cache.Items.InitCatalogItems();
            Habbo.Cache.Models.InitModels();
            Habbo.Cache.FurniTure.InitItems();
            #endregion
            #region StartSockets
            gameSocketServer.SetupSocket(GamePort, ConnectionsLimits);
            #endregion
            //#region MUS
            //Sockets.TCPServer.startMUS(30003);
            //#endregion

            UserManager.Init();
            Thread WorkerVoid = new Thread(new ThreadStart(Worker.Process));
            WorkerVoid.Start();

            Out.WritePlain("Awaiting Connections...", ConsoleColor.White);
            Console.Title = "Zazlak";
            Console.Beep();

            while (true)
            {
                if (!Kernel.Core.ConsoleCommands.Logged)
                    Kernel.Core.ConsoleCommands.Parse(Console.ReadLine());
                else
                    Kernel.Core.ConsoleCommands.LOL(Console.ReadLine());
            }
        }

        public static bool EnumToBool(DataRow Enum)
        {
            if (Enum.ToString() == "1")
            {
                return true;
            }

            return false;
        }

        public class Worker
        {
            public static void Process()
            {
                while (true)
                {
                    Thread.Sleep(5000);
                }
            }
        }
    }
}
