using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Zazlak.Storage;

namespace Zazlak.Habbo.Responses
{
    class HabboNavigator
    {
        internal User User;

        public HabboNavigator(User User)
        {
            this.User = User;
        }

        internal void NavigatorRooms()
        {
            #region Navigator - ROOMS
            /*
            ServerMessage Rooms = new ServerMessage(Headers.SendAllRooms);
            Rooms.AppendInt32(5); // ??
            Rooms.AppendString("");

            int Category = int.Parse(User.ActualClientMessage.GetNextString());

            // Rooms with most visitors
            List<RoomData> RoomsToSearch = new List<RoomData>();
            if (Category == -1)
                RoomsToSearch = RoomData.AllRoom;
            else if (Category == 0)
                RoomsToSearch = RoomData.GetRoomsForCategory(8);
            else
                RoomsToSearch = RoomData.GetRoomsForCategory(Category);
            List<RoomData> zRoom = new List<RoomData>();
            List<RoomData> Room = new List<RoomData>();
            int CurrentMax = 0;
            foreach (RoomData Data in RoomsToSearch)
            {
                // I must code this later uh...
                if (Data.CurrentUsersInRoom > 0)
                {
                    if (Data.CurrentUsersInRoom > CurrentMax)
                        CurrentMax = Data.CurrentUsersInRoom;
                    zRoom.Add(Data);
                }
            }

            for (int i = 0; CurrentMax != i; )
            {
                foreach (RoomData Data in zRoom)
                {
                    if (Data.CurrentUsersInRoom == CurrentMax)
                        Room.Add(Data);
                    Console.WriteLine(Data);
                }
                CurrentMax--;
            }

            Rooms.AppendInt32(Room.Count);
            foreach (RoomData Data in RoomData.AllRoom)
            {
                Rooms.AppendInt32(Data.Id);
                Rooms.AppendBoolean(false); // events
                Rooms.AppendString(Data.Name);
                Rooms.AppendInt32(Data.OwnerId);
                Rooms.AppendString(User.GetUserData().UserName);
                Rooms.AppendInt32(0);
                Rooms.AppendInt32(Data.CurrentUsersInRoom);
                Rooms.AppendInt32(Data.MaxUsers);
                Rooms.AppendString(Data.Description);
                Rooms.AppendInt32(0);
                Rooms.AppendInt32((Data.Category == 3) ? 0 : 2);
                Rooms.AppendInt32(Data.Score);
                Rooms.AppendInt32(Data.Category);
                Rooms.AppendString("");
                Rooms.AppendInt32(0);
                Rooms.AppendInt32(0);
                Rooms.AppendInt32(Data.TagList.Count);
                foreach (string D in Data.TagList)
                    Rooms.AppendString(D);

                Rooms.AppendInt32(0);
                Rooms.AppendInt32(0);
                Rooms.AppendInt32(0);

                // booleans
                Rooms.AppendBoolean(true);
                Rooms.AppendBoolean(true);
            }
            Rooms.AppendBoolean(false);
            User.SendMessage(Rooms);
             */
            #endregion
        }

        internal void MyRooms()
        {
            #region Navigator - ME
            ServerMessage MyRooms = new ServerMessage(Headers.MyRoomsInit);
            MySQL MyRoomsSQL = new MySQL(Init.ServerMySQL);
            MyRoomsSQL.Query("SELECT * FROM rooms WHERE ownerid = '" + (int)User.HabboUser.UserRow["Id"] + "'");
            MyRooms.AppendInt32(5);
            MyRooms.AppendString("");
            MyRooms.AppendInt32(MyRoomsSQL.Num_Rows()); // Numero de salas

            foreach (DataRow Data in MyRoomsSQL.Fetch_Array())
            {
                List<string> TagList;
                TagList = new List<string>();
                if (Data["tags"].ToString().Contains(";"))
                {
                    string[] Separe = Data["tags"].ToString().Split(';');
                    foreach (string s in Separe)
                    {
                        TagList.Add(s);
                    }
                }

                MyRooms.AppendInt32((int)Data["id"]);
                MyRooms.AppendBoolean(false); // events
                MyRooms.AppendString((string)Data["name"]);
                MyRooms.AppendInt32((int)Data["ownerid"]);
                MyRooms.AppendString((string)User.HabboUser.UserRow["username"]);
                MyRooms.AppendInt32(0);
                MyRooms.AppendInt32((int)Data["usersnow"]);
                MyRooms.AppendInt32((int)Data["maxusers"]);
                MyRooms.AppendString((string)Data["description"]);
                MyRooms.AppendInt32(0);
                MyRooms.AppendInt32((Convert.ToInt32(Data["category"]) == 3) ? 0 : 2);
                MyRooms.AppendInt32((int)Data["score"]);
                MyRooms.AppendInt32(Convert.ToInt32(Data["category"]));
                MyRooms.AppendString("");
                MyRooms.AppendInt32(0);
                MyRooms.AppendInt32(0);
                MyRooms.AppendInt32(TagList.Count);
                foreach (string D in TagList)
                    MyRooms.AppendString(D);
                MyRooms.AppendInt32(0);
                MyRooms.AppendInt32(0);
                MyRooms.AppendInt32(0);

                // booleans
                MyRooms.AppendBoolean(true);
                MyRooms.AppendBoolean(true);
            }
            MyRooms.AppendBoolean(false);
            User.sendPacket(MyRooms);
            #endregion
        }

        internal void Search()
        {
            #region Navigator - SEARCH
//[0][0][0][13]¡[0][0][0][8][0][9]owner:AQS[0][0][0][1][0]dßÕ[0][0][AQS] -> Chill|[0][5]°[3][0][3]AQS[0][0][0][0]
//[0][0][0][0][0][0][0][0]http://habboway.dk/[0][0][0][0][0][0][0][2][0][0][0][0]
//[0][0]5[0][0][0][0][0][0][0][0][0][0][0][0][0][0][0][0][0][0][0][0][0][0][0][0][0][0][1][1][0]
            //ServerMessage Search = new ServerMessage(Headers.MyRoomsInit);
            
            /*
            Dictionary<string, int> Tags = new Dictionary<string, int>();

                //DataTable Data = MySQL.query_read("SELECT tags FROM rooms WHERE public_room = '0'");
                MySQL Data = new MySQL(Init.ServerMySQL);
                Data.Query("SELECT tags FROM rooms WHERE public_room = '0'");
            
                if (Data != null)
                {
                    foreach (DataRow Row in Data.Fetch_Array())
                    {
                        List<string> RoomTags = new List<string>();

                        foreach (string Tag in Row["tags"].ToString().Split(';'))
                        {
                            RoomTags.Add(Tag);
                        }

                        foreach (RoomData UsersNow in RoomData.AllRoom)
                        {
                            foreach (string Tag in RoomTags)
                            {
                                if (Tags.ContainsKey(Tag))
                                {
                                    Tags[Tag] += UsersNow.CurrentUsersInRoom;
                                }
                                else
                                {
                                    Tags.Add(Tag, UsersNow.CurrentUsersInRoom);
                                }
                            }
                        }
                    }
                }
                else
                {
                    return;
                }

                List<KeyValuePair<string, int>> SortedTags = new List<KeyValuePair<string, int>>(Tags);

                SortedTags.Sort(

                    delegate(KeyValuePair<string, int> firstPair,

                    KeyValuePair<string, int> nextPair)
                    {
                        return firstPair.Value.CompareTo(nextPair.Value);
                    }

                );

                ServerMessage Message = new ServerMessage(PacketHeaders.NAVEGADORSEARCH);
                Message.AppendInt32(SortedTags.Count);

                foreach (KeyValuePair<string, int> TagData in SortedTags)
                {
                    Message.AppendString(TagData.Key);
                    Message.AppendInt32(TagData.Value);
                }

                User.SendMessage(Message);
             */
            #endregion
        }

        internal void SearhRooms()
        {
            #region Packet
            /*
            string Name = User.ActualClientMessage.GetNextString();
            ServerMessage Search = new ServerMessage(Headers.SearchRoomsName);

            List<DataTable> RoomsSearch = new List<DataTable>();
            bool searchforuser = false;

            Search.AppendInt32(8);

            foreach (UsersData Uzer in UsersData.UsersRegistered)
            {
                if (Uzer.UserName == Name)
                    searchforuser = true;
            }
            if (searchforuser)
            {
                foreach (RoomData Data in RoomData.AllRoom)
                {
                    if (User.GetUserData().UserName == Name)
                        RoomsSearch.Add(Data);
                }
            }
            else
            {
                foreach (RoomData Data in RoomData.AllRoom)
                {
                    if (Data.Name.Contains(Name))
                        RoomsSearch.Add(Data);
                }
            }
            Search.AppendString(Name);
            Search.AppendInt32(RoomsSearch.Count);
            foreach (RoomData Data in RoomData.AllRoom)
            {
                Search.AppendInt32(Data.Id);
                Search.AppendBoolean(false); // events
                Search.AppendString(Data.Name);
                Search.AppendInt32(Data.OwnerId);
                Search.AppendString(User.GetUserData().UserName);
                Search.AppendInt32(0);
                Search.AppendInt32(Data.CurrentUsersInRoom);
                Search.AppendInt32(Data.MaxUsers);
                Search.AppendString(Data.Description);
                Search.AppendInt32(0);
                Search.AppendInt32((Data.Category == 3) ? 0 : 2);
                Search.AppendInt32(Data.Score);
                Search.AppendInt32(Data.Category);
                Search.AppendString("");
                Search.AppendInt32(0);
                Search.AppendInt32(0);
                Search.AppendInt32(Data.TagList.Count);
                foreach (string D in Data.TagList)
                    Search.AppendString(D);

                Search.AppendInt32(0);
                Search.AppendInt32(0);
                Search.AppendInt32(0);

                // booleans
                Search.AppendBoolean(true);
                Search.AppendBoolean(true);
            }
            Search.AppendBoolean(false);
            User.SendMessage(Search);
*/
            #endregion
        }
    }
}
