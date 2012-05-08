using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Zazlak.Sockets;
using System.Threading;
using Zazlak.Habbo.Responses;
using Zazlak.Habbo;
namespace Zazlak.Habbo
{
    class User
    {
        private gameConnection Game;

        internal bool pingOK;
        internal int connectionID;

        internal ClientMessage ActualClientMessage;
        internal string ActualPacket = "";

        internal HabboUser HabboUser;
        internal HabboCatalog HabboCatalog;
        internal HabboNavigator HabboNavigator;
        internal HabboRooms HabboRooms;

        #region Constructors/destructors
        public User(gameConnection ConnectionManager)
        {
            this.Game = ConnectionManager;
            this.connectionID = ConnectionManager.ConnectionID;
            this.HabboUser = new HabboUser(this);
            this.HabboCatalog = new HabboCatalog(this);
            this.HabboNavigator = new HabboNavigator(this);
            this.HabboRooms = new HabboRooms(this);
        }
        #endregion

        internal void Disconnect()
        {
            Game.Close();
        }


        internal void checkPings()
        {
            while (true)
            {
               this.Game.Ping();
            }
        }

        internal void Reset()
        {
        }

        private void delDisconnectTimed(int ms)
        {
            Thread.Sleep(ms);
            Disconnect();
        }

        internal void sendPacket(ServerMessage Message)
        {
            if (Message.ToString().Length > 0)
            {
                if (Message == null)
                {
                    return;
                }

                Game.sendData(Message.ToBytes());
            }
        }

        internal void sendPacket(BuildMessage Message)
        {
            if (Message.ToString().Length > 0)
            {
                if (Message == null)
                {
                    return;
                }

                Game.sendData(Message.ToBytes());
            }
        }

        public string IP
        {
            get
            {
                return Game.connectionRemoteIP;
            }
        }
    }
}
