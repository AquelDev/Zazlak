using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Zazlak.Storage;
using Zazlak.Sockets;

namespace Zazlak.Habbo.Responses
{
    class HabboRooms
    {
        internal User User;

        public HabboRooms(User User)
        {
            this.User = User;
        }

        public DataRow GetRoom(int RoomId)
        {
            MySQL RoomsRow = new MySQL(Init.ServerMySQL);
            RoomsRow.Query("SELECT * FROM rooms WHERE id = '" + RoomId + "' LIMIT 1");
            return RoomsRow.Fetch_Assoc();
        }

        internal void LoadRoom()
        {
            int RoomId = User.ActualClientMessage.GetNextInt();

            MySQL RoomsRow = new MySQL(Init.ServerMySQL);
            RoomsRow.Query("SELECT * FROM rooms WHERE id = '" + RoomId + "' LIMIT 1");
            Out.WriteLine(RoomsRow.Num_Rows());
            if (RoomsRow.Num_Rows() == 1)
            {
                DataRow RD = RoomsRow.Fetch_Assoc();

                User.HabboUser.CurrentRoomId = RoomId;
                //Habbo.VisitedRooms += RoomId + ";";
                //Habbo.IsOnRoom = true;

                BuildMessage Load = new BuildMessage();
                ServerMessage LoadRooms = new ServerMessage(Headers.LOADROOMS);
                Load.AppendPacket(LoadRooms);

                ServerMessage LoadRooms2 = new ServerMessage(1059);
                LoadRooms2.AppendInt32(77824);
                LoadRooms2.AppendInt32(513);
                Load.AppendPacket(LoadRooms2);

                ServerMessage Model = new ServerMessage(Headers.LOADMODEL);
                Model.AppendString(RD["model"].ToString());
                Model.AppendInt32(RoomId);
                Load.AppendPacket(Model);

                if (RD["wall"].ToString() != "0")
                {
                    ServerMessage SendWallpaper = new ServerMessage(Headers.LOADWALLSFLOORSLANDS);
                    SendWallpaper.AppendString("wallpaper");
                    SendWallpaper.AppendString(RD["wall"]);
                    Load.AppendPacket(SendWallpaper);
                }

                if (RD["floor"].ToString() != "0")
                {
                    ServerMessage SendFloor = new ServerMessage(Headers.LOADWALLSFLOORSLANDS);
                    SendFloor.AppendString("floor");
                    SendFloor.AppendString(RD["floor"]);
                    Load.AppendPacket(SendFloor);
                }

                ServerMessage SendLandscape = new ServerMessage(Headers.LOADWALLSFLOORSLANDS);
                SendLandscape.AppendString("landscape");
                SendLandscape.AppendString(RD["landscape"] + ".0");
                Load.AppendPacket(SendLandscape);

                ServerMessage Unk = new ServerMessage(Headers.UNK);
                Unk.AppendInt32(4);
                Load.AppendPacket(Unk);

                if (RD["OwnerId"] == User.HabboUser.UserRow["id"])
                {
                    ServerMessage LoadPows = new ServerMessage(Headers.LOADPOWS);
                    Load.AppendPacket(LoadPows);
                }

                ServerMessage LoadScore = new ServerMessage(Headers.LOADSCORE);
                LoadScore.AppendInt32((int)RD["score"]);
                LoadScore.AppendBoolean(false);
                Load.AppendPacket(LoadScore);

                ServerMessage LoadEvent = new ServerMessage(Headers.LOADEVENTS);
                LoadEvent.AppendString("-1");
                Load.AppendPacket(LoadEvent);

                //Habbo.Cache.Rooms;
                User.sendPacket(Load);
            }
        }

        internal void LoadModel()
        {
            try
            {
                BuildMessage CollectMaps = new BuildMessage();
                DataRow Row = GetRoom(User.HabboUser.CurrentRoomId);
                Cache.Models GetModel = Cache.Models.RoomModelByName[Row["model"].ToString()];

                ServerMessage ModelHeightMap = new ServerMessage(Headers.HEIGHTMAP);
                ModelHeightMap.AppendString(GetModel.GetPremairParams());
                CollectMaps.AppendPacket(ModelHeightMap);

                ServerMessage ModelAnothers = new ServerMessage(Headers.ANOTHERMAP);
                ModelAnothers.AppendString(GetModel.GetSecondairParams());
                CollectMaps.AppendPacket(ModelAnothers);
                User.sendPacket(CollectMaps);
            }
            catch
            { }
        }

        internal void ThirdRequest()
        {
            DataRow Row = GetRoom(User.HabboUser.CurrentRoomId);

            BuildMessage NewBuild = new BuildMessage();

            ServerMessage First = new ServerMessage(Headers.FIRSTP);
            First.AppendInt32(0);
            NewBuild.AppendPacket(First);

            ServerMessage First1 = new ServerMessage(Headers.FIRST1);
            First1.AppendInt32(0);
            NewBuild.AppendPacket(First1);

            //Data.UserList.Add(Habbo);
            //Data.CurrentUsersInRoom++;
            User.HabboUser.X = Cache.Models.RoomModelByName[Row["model"].ToString()].DoorX;
            User.HabboUser.Y = Cache.Models.RoomModelByName[Row["model"].ToString()].DoorY;
            User.HabboUser.Z = "0.0";

            ServerMessage First2 = new ServerMessage(Headers.WALLITEMS);
            First2.AppendInt32(0);
            First2.AppendInt32(0);
            NewBuild.AppendPacket(First2);

            ServerMessage First3 = new ServerMessage(Headers.FLOORITEMS);
            First3.AppendInt32(0);
            First3.AppendInt32(0);
            NewBuild.AppendPacket(First3);

            ServerMessage SerializeUser = new ServerMessage(Headers.SERIALIZEUSER);
            SerializeUser.AppendInt32(1);

            foreach (User Rowf in UserManager._Users.Values)
           {
                //UsersData UserData = tUser.GetUserData();
                SerializeUser.AppendInt32(Convert.ToInt32(Rowf.HabboUser.UserRow["id"]));
                SerializeUser.AppendString(Convert.ToString(Rowf.HabboUser.UserRow["username"]));
                SerializeUser.AppendString(Convert.ToString(Rowf.HabboUser.UserRow["motto"]));
                SerializeUser.AppendString(Convert.ToString(Rowf.HabboUser.UserRow["figure"]));
                SerializeUser.AppendInt32(0); // Rot
                SerializeUser.AppendInt32(Rowf.HabboUser.X);
                SerializeUser.AppendInt32(Rowf.HabboUser.Y);
                SerializeUser.AppendString(Rowf.HabboUser.Z);
                SerializeUser.AppendInt32(2);
                SerializeUser.AppendInt32(1);
                SerializeUser.AppendString(Convert.ToString(Rowf.HabboUser.UserRow["gender"]).ToLower());
                SerializeUser.AppendBreak();
                SerializeUser.AppendBreak();
                SerializeUser.AppendInt32(0);
                SerializeUser.AppendInt32(460); // Score points
            }
            NewBuild.AppendPacket(SerializeUser);

            ServerMessage Extra1 = new ServerMessage(Headers.EXTRA1);
            Extra1.AppendBoolean(false);
            Extra1.AppendInt32(0);
            Extra1.AppendInt32(0);
            NewBuild.AppendPacket(Extra1);

            ServerMessage Extra2 = new ServerMessage(Headers.EXTRA2);
            Extra2.AppendBoolean(true);
            Extra2.AppendInt32(User.HabboUser.CurrentRoomId);
            Extra2.AppendBoolean(true);
            NewBuild.AppendPacket(Extra2);

            User.sendPacket(NewBuild);
        }

        internal void FourthRequest()
        {
            DataRow Row = GetRoom(User.HabboUser.CurrentRoomId);

            BuildMessage NewBuild = new BuildMessage();

            ServerMessage Extra3 = new ServerMessage(Headers.INSERTU);
            foreach (User Rowf in UserManager._Users.Values)
            {
                Extra3.AppendInt32(1);
                Extra3.AppendInt32(Rowf.connectionID);
                Extra3.AppendInt32(Rowf.HabboUser.X);
                Extra3.AppendInt32(Rowf.HabboUser.Y);
                Extra3.AppendString(Rowf.HabboUser.Z);
                Extra3.AppendInt32(2);
                Extra3.AppendInt32(2);
                Extra3.AppendString("/flatctrl 4//");
            }
            NewBuild.AppendPacket(Extra3);

            ServerMessage Extra4 = new ServerMessage(Headers.EXTRA4);
            Extra4.AppendInt32(0);
            NewBuild.AppendPacket(Extra4);

            ServerMessage Message = new ServerMessage(Headers.ROOMINFOO);
            Message.AppendBoolean(true);
            Message.AppendInt32((int)Row["id"]);
            Message.AppendBoolean(false); // events
            Message.AppendString((string)Row["name"]);
            Message.AppendInt32((int)Row["ownerid"]);
            Message.AppendString("Xdr");
            Message.AppendInt32(0);
            Message.AppendInt32((int)Row["usersnow"]);
            Message.AppendInt32(Row["id"]);
            Message.AppendString(Row["description"]);
            Message.AppendInt32(0);
            Message.AppendInt32((Convert.ToInt32(Row["category"]) == 3) ? 0 : 2);
            Message.AppendInt32(Row["score"]);
            Message.AppendInt32(Row["category"]);
            Message.AppendString("");
            Message.AppendInt32(0);
            Message.AppendInt32(0);
            Message.AppendInt32(0); // tags count
            //foreach (string D in Data.TagList)
              //  Message.AppendString(D);

            Message.AppendInt32(0);
            Message.AppendInt32(0);
            Message.AppendInt32(0);

            // booleans
            Message.AppendBoolean(true);
            Message.AppendBoolean(true);
            Message.AppendBoolean(false);
            Message.AppendBoolean(false);
            Message.AppendBoolean(false);
            NewBuild.AppendPacket(Message);

            User.sendPacket(NewBuild);
        }

        internal void Speak()
        {
            string Message = User.ActualClientMessage.GetNextString();

            /*
            Commands Command = new Commands(Habbo, Message);
            bool notsendpacket = false;
            if (Message.StartsWith(":") && Command.ThereIsACommand(Message))
            {
                Command.ProcessCommands();
                notsendpacket = true;
            }
            */
            bool notsendpacket = false;

            if (!notsendpacket)
            {
                ServerMessage SPEAK = new ServerMessage(1621);
                SPEAK.AppendInt32(0); // Veces iniciada la sala? ¿A cada usuario de sala se le acredita un id?
                SPEAK.AppendString(Message); // Mensaje
                SPEAK.AppendInt32(0);
                SPEAK.AppendInt32(0);
                SPEAK.AppendInt32(0); // Mensajes mandados en la sala por ese usuario
                //StaticGame.SendPacketToAllRoom(Habbo, SPEAK);
                User.sendPacket(SPEAK);
            }
        }
    }
}
