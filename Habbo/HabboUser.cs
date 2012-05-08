using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using Zazlak.Habbo;
using Zazlak.Habbo.Responses;
using Zazlak.Sockets;
using Zazlak.Storage;

namespace Zazlak.Habbo
{
    class HabboUser
    {
        internal User User;
        internal DataRow UserRow;
        internal DataRow Row;

        internal int CurrentRoomId = 0;

        internal int X = 0;
        internal int Y = 0;
        internal string Z = "";
        internal int DanceId = 0;
        internal bool Connected;

        private static Thread pingChecker;

        public HabboUser(User Info)
        {
            this.User = Info;
            int UserId = 1;

            MySQL Lol = new MySQL(Init.ServerMySQL);
            Lol.Query("SELECT * FROM users_characters WHERE Id = '" + UserId + "'");
            this.UserRow = Lol.Fetch_Assoc();
        }

        #region Packets
        internal void Login()
        {
            
            ServerMessage SendLogin = new ServerMessage(Headers.ClientInit);
            this.User.sendPacket(SendLogin);

            UserManager.addUser((int)this.UserRow["id"], User);

            pingChecker = new Thread(new ThreadStart(User.checkPings));
            pingChecker.Priority = ThreadPriority.Lowest;
            pingChecker.Start();

        }

        internal void Stream()
        {
            ServerMessage Streaming = new ServerMessage(1085);
            Streaming.AppendInt32(1);
            Streaming.AppendInt32(1);
            Streaming.AppendInt32(4);
            Streaming.AppendString("-1");
            Streaming.AppendString("");
            Streaming.AppendString("null");
            Streaming.AppendString("");
            Streaming.AppendInt32(5);
            Streaming.AppendInt32(6);
            Streaming.AppendBoolean(false);
            Streaming.AppendInt32(1);
            Streaming.AppendString("");
            Streaming.AppendString("Test");
            Streaming.AppendString("YesTestLol");
            Streaming.AppendString("");
            this.User.sendPacket(Streaming);
        }

        internal void Chatting()
        {
            //[LOG] > CLIENT ID: '3789': [0][0][0]Í[0]¾ý[0][4]hey.[0][0]
            /*int One = User.ActualClientMessage.GetNextInt();
            Console.WriteLine(One);*/
            ServerMessage chat = new ServerMessage(3789);
            chat.AppendInt32(User.ActualClientMessage.GetNextInt());
            chat.AppendString("Hey! :)");
            chat.AppendBoolean(false);
            chat.AppendBoolean(false);
            this.User.sendPacket(chat);
        }

        internal void sendPacket()
        {
            #region Packet
            ServerMessage SendCredits = new ServerMessage(Headers.SendCredits);
            SendCredits.AppendString(Convert.ToInt32(UserRow["credits"]) + ".0");
            this.User.sendPacket(SendCredits);

            ServerMessage SendUser = new ServerMessage(Headers.SendUserData);
            SendUser.AppendInt32(Convert.ToInt32(UserRow["id"]));
            SendUser.AppendString(Convert.ToString(UserRow["username"]));
            SendUser.AppendString("hr-155-32.hd-185-2.ch-3030-62.lg-275-62.sh-290-62,s-0.g-1.d-3.h-3.a-0");
            SendUser.AppendString(Convert.ToString(UserRow["gender"]).ToLower());
            SendUser.AppendString(Convert.ToString(UserRow["motto"]));
            SendUser.AppendString("habbowall");
            SendUser.AppendInt32(0);
            SendUser.AppendBoolean(false);
            SendUser.AppendInt32(0); // Friends
            SendUser.AppendInt32(3);
            SendUser.AppendInt32(3);
            SendUser.AppendBoolean(true);
            SendUser.AppendString("02-05-2012 08:05:10");
            this.User.sendPacket(SendUser);

            ServerMessage Friends = new ServerMessage(Headers.FriendsBarInit);
            Friends.AppendInt32(100); // amount friends normal
            Friends.AppendInt32(100); // amount max friends normal
            Friends.AppendInt32(200); // amount max friends HC?
            Friends.AppendInt32(300); // amount max friends VIP?

            Friends.AppendInt32(0); // amount categorie

            MySQL FriendSQL = new MySQL(Init.ServerMySQL);
            FriendSQL.Query("SELECT user_two_id FROM users_friends WHERE user_one_id = '"+UserRow["id"]+"'");

            Friends.AppendInt32(FriendSQL.Num_Rows()); // amount friends

            foreach (DataRow TheRow in FriendSQL.Fetch_Array())
            {
                MySQL TheFriend = new MySQL(Init.ServerMySQL);
                TheFriend.Query("SELECT * FROM users_characters WHERE id = '"+TheRow["user_two_id"]+"'");
                this.Row = TheFriend.Fetch_Assoc();

                if (Row["connected"].ToString() == "1")
                {
                    Connected = true;
                }
                else
                {
                    Connected = false;
                }
                

                Friends.AppendInt32(Row["id"]); // id
                Friends.AppendString(Row["username"]); // name
                Friends.AppendInt32(1); // ?
                Friends.AppendBoolean(Connected); // isonline
                Friends.AppendBoolean(false); // isinroom
                Friends.AppendString(Row["figure"]); // figure
                Friends.AppendInt32(0); // ?
                Friends.AppendString(Row["motto"]); // motto
                Friends.AppendString("Mikkel"); // facebook name
                Friends.AppendString(Row["createdon"]);
                Friends.AppendBoolean(false);
            }

            Friends.AppendInt32(100); // ?
            Friends.AppendInt32(0); // ?
            this.User.sendPacket(Friends);

            ServerMessage Club = new ServerMessage(Headers.VipInit);
            Club.AppendString("club_habbo");
            Club.AppendInt32(10); // Dias
            Club.AppendInt32(0);
            Club.AppendInt32(0);
            Club.AppendInt32(1);
            Club.AppendBoolean(false);
            Club.AppendBoolean(true);
            Club.AppendInt32(0);
            Club.AppendInt32(0);
            Club.AppendInt32(0);
            this.User.sendPacket(Club);

            ServerMessage SendPacket = new ServerMessage(2994);
            SendPacket.AppendString("Hola");
            SendPacket.AppendBoolean(false);
            SendPacket.AppendBoolean(false);
            this.User.sendPacket(SendPacket);

            ServerMessage SendPixels = new ServerMessage(Headers.SendPixels);
            SendPixels.AppendInt32(1);
            SendPixels.AppendInt32(0);
            SendPixels.AppendInt32(UserRow["pixels"]);
            this.User.sendPacket(SendPixels);
            #endregion
        }

        internal void HomeRoom()
        {
            #region Packet
            if ((int)this.UserRow["homeRoom"] != 0)
            {
                ServerMessage EnterHomeRoom = new ServerMessage(688);
                EnterHomeRoom.AppendInt32(this.UserRow["homeRoom"]);
                EnterHomeRoom.AppendInt32(this.UserRow["homeRoom"]);
                User.sendPacket(EnterHomeRoom);
            }
            #endregion
        }

        internal void UpdateLook()
        {
            #region Packet
            string Gender = "M";

            if (User.ActualClientMessage.GetNextString() == "F")
                Gender = "F";

            string NewLook = User.ActualClientMessage.GetNextString();

            MySQL Lol = new MySQL(Init.ServerMySQL);
            Lol.Query("UPDATE users_characters SET figure = '" + NewLook + "', gender = '" + Gender + "' WHERE id = '" + this.UserRow["id"] + "'");

            this.UserRow["figure"] = NewLook;
            this.UserRow["gender"] = Gender;

            ServerMessage SUpdateInRoom = new ServerMessage(1567);
            SUpdateInRoom.AppendBreak();
            SUpdateInRoom.AppendString(NewLook);
            SUpdateInRoom.AppendString(Gender.ToLower());
            SUpdateInRoom.AppendString(this.UserRow["motto"]);
            SUpdateInRoom.AppendInt32(0);
            User.sendPacket(SUpdateInRoom);

            /*
            ServerMessage SUpdateInRoom2 = new ServerMessage(1567);
            SUpdateInRoom.AppendInt32(User.connectionID);
            SUpdateInRoom.AppendString(UserRow["figure"]);
            SUpdateInRoom.AppendString(UserRow["gender"].ToString().ToLower());
            SUpdateInRoom2.AppendString(this.UserRow["motto"]);
            SUpdateInRoom2.AppendInt32(0);
            //StaticGame.SendPacketToAllRoom(this, SUpdateInRoom);
            User.sendPacket(SUpdateInRoom2);
            */
            #endregion
        }

        internal void Dance()
        {
            #region Packet
            int DanceId = User.ActualClientMessage.GetNextInt();

            if (DanceId != 0)
            {
                User.HabboUser.DanceId = DanceId;
            }
            else
            {
                User.HabboUser.DanceId = 0;
            }

            ServerMessage Dance = new ServerMessage(433);
            Dance.AppendInt32(User.connectionID);
            Dance.AppendInt32(DanceId);
            User.sendPacket(Dance);
            #endregion
        }

        internal void Sit()
        {
            #region Packet
            ServerMessage Sit = new ServerMessage(2268);
            Sit.AppendInt32(1);
            Sit.AppendInt32(User.connectionID); // Virtual ID! ¡AARRGG!
            Sit.AppendInt32(User.HabboUser.X);
            Sit.AppendInt32(User.HabboUser.Y);
            Sit.AppendString(User.HabboUser.Z);
            Sit.AppendInt32(2);
            Sit.AppendInt32(2);
            Sit.AppendString("/flatctrl 4/sit 0.55 1/");
            //StaticGame.SendPacketToAllRoom(Habbo, Sit);
            User.sendPacket(Sit);
            #endregion
        }

        internal void Sings()
        {
            #region Packet
            int SingType = User.ActualClientMessage.GetNextInt();

            ServerMessage Sing1 = new ServerMessage(Headers.Sing);
            Sing1.AppendInt32(1);
            Sing1.AppendInt32(User.connectionID);
            Sing1.AppendInt32(this.X);
            Sing1.AppendInt32(this.Y);
            Sing1.AppendString(this.Z);
            Sing1.AppendInt32(2);
            Sing1.AppendInt32(2);
            Sing1.AppendString("/flatctrl 4//sign " + SingType + "/");
            User.sendPacket(Sing1);

            ServerMessage Sing2 = new ServerMessage(Headers.Sing);
            Sing2.AppendInt32(1);
            Sing2.AppendInt32(User.connectionID);
            Sing2.AppendInt32(this.X);
            Sing2.AppendInt32(this.Y);
            Sing2.AppendString(this.Z);
            Sing2.AppendInt32(2);
            Sing2.AppendInt32(2);
            Sing2.AppendString("/flatctrl 4//");
            User.sendPacket(Sing2);
            #endregion
        }

        internal void Wave()
        {
            #region Packet
            int WaveId = User.ActualClientMessage.GetNextInt();

            ServerMessage Wave = new ServerMessage(1496);
            Wave.AppendInt32(User.connectionID);
            Wave.AppendInt32(WaveId);
            User.sendPacket(Wave);
            #endregion
        }

        internal void Idle()
        {
            #region Packet
            int IdleId = User.ActualClientMessage.GetNextInt();

            ServerMessage Idle = new ServerMessage(1496);
            Idle.AppendInt32(User.connectionID);
            Idle.AppendInt32(IdleId);
            //StaticGame.SendPacketToAllRoom(Habbo, Idle);
            User.sendPacket(Idle);
            ServerMessage Idle2 = new ServerMessage(1028);
            Idle2.AppendInt32(User.connectionID);
            Idle2.AppendBoolean(true);
            User.sendPacket(Idle2);
            //StaticGame.SendPacketToAllRoom(Habbo, Idle2);
#endregion
        }

        internal void ChangeMotto()
        {
            #region Packet
            this.UserRow["motto"] = User.ActualClientMessage.GetNextString();

            MySQL Lol = new MySQL(Init.ServerMySQL);
            Lol.Query("UPDATE users_characters SET motto = '" + UserRow["motto"] + "' WHERE id = '" + this.UserRow["id"] + "'");

            ServerMessage SUpdateInRoom = new ServerMessage(1567);
            SUpdateInRoom.AppendInt32(-1);
            SUpdateInRoom.AppendString(UserRow["figure"]);
            SUpdateInRoom.AppendString(UserRow["gender"].ToString().ToLower());
            SUpdateInRoom.AppendString(UserRow["motto"]);
            SUpdateInRoom.AppendInt32(485);
            User.sendPacket(SUpdateInRoom);

            
            ServerMessage SUpdateInRoom2 = new ServerMessage(1567);
            SUpdateInRoom.AppendInt32(0);
            SUpdateInRoom.AppendString(UserRow["figure"]);
            SUpdateInRoom.AppendString(UserRow["gender"].ToString().ToLower());
            SUpdateInRoom2.AppendString(this.UserRow["motto"]);
            SUpdateInRoom2.AppendInt32(485);
            User.sendPacket(SUpdateInRoom2);
            
            Console.WriteLine(this.UserRow["motto"]);
            //StaticGame.SendPacketToAllRoom(this, SUpdateInRoom);
            #endregion
        }

        internal void UserProfile()
        {
            int UserId = User.ActualClientMessage.GetNextInt();
            MySQL Lol = new MySQL(Init.ServerMySQL);
            Lol.Query("SELECT * FROM users_characters WHERE Id = '" + UserId + "'");
            this.UserRow = Lol.Fetch_Assoc();

            if (UserRow["connected"].ToString() == "1")
            {
                Connected = true;
            }
            else
            {
                Connected = false;
            }
                
           
            ServerMessage Profile = new ServerMessage(Headers.SendProfile);
            Profile.AppendInt32(UserRow["id"]);
            Profile.AppendString(UserRow["username"]);
            Profile.AppendString(UserRow["figure"]);
            Profile.AppendString(UserRow["motto"]);
            Profile.AppendString(UserRow["createdon"]);
            Profile.AppendInt32(1337);
            Profile.AppendInt32(2);
            Profile.AppendBoolean(false);
            Profile.AppendBoolean(false);
            Profile.AppendBoolean(Connected);
            Profile.AppendInt32(0);
            Profile.AppendInt32(((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds - (int)User.HabboUser.UserRow["lastaccess"]));
            Profile.AppendBoolean(true);
            this.User.sendPacket(Profile);

            ServerMessage SendCredits = new ServerMessage(Headers.SendCredits);
            SendCredits.AppendString("1000.0");
            this.User.sendPacket(SendCredits);
        }

        #endregion
    }
}
