using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Zazlak.Habbo.Cache
{
    class Rooms
    {
        //public static List<Rooms> RoomsData;
        //public static Dictionary<string, Rooms> RoomsId;

        internal UInt32 Id;
        internal string Name;
        internal string Description;
        internal string Type;
        internal string Owner;
        internal string Password;
        internal int State;
        internal int Category;
        internal int UsersNow;
        internal int UsersMax;
        internal string ModelName;
        internal string CCTs;
        internal int Score;
        internal List<string> Tags;
        internal bool AllowPets;
        internal bool AllowPetsEating;
        internal bool AllowWalkthrough;
        internal bool AllowRightsOverride;
        internal bool Hidewall;
        //private RoomIcon myIcon;
        //internal RoomEvent Event;
        internal string Wallpaper;
        internal string Floor;
        internal string Landscape;
        private Models mModel;

        internal Boolean IsPublicRoom
        {
            get
            {
                if (Type.ToLower() == "public")
                {
                    return true;
                }

                return false;
            }
        }
        /*
        internal RoomIcon Icon
        {
            get
            {
                return myIcon;
            }
        }*/

        internal int TagCount
        {
            get
            {
                return Tags.Count;
            }
        }
        /*
        internal Models Model
        {
            get
            {
                if (mModel == null)
                    mModel = InfoEnv.GetGame().GetRoomManager().GetModel(ModelName, Id);
                return mModel;
            }
        }*/

        internal Rooms() { }

        internal void Fill(DataRow Row)
        {
            this.Id = (UInt32)Row["id"];
            this.Name = (string)Row["caption"];
            this.Description = (string)Row["description"];
            this.Type = (string)Row["roomtype"];
            this.Owner = (string)Row["owner"];

            switch (Row["state"].ToString().ToLower())
            {
                case "open":

                    this.State = 0;
                    break;

                case "password":

                    this.State = 2;
                    break;

                case "locked":
                default:

                    this.State = 1;
                    break;
            }

            this.Category = (int)Row["category"];
            this.UsersNow = (int)Row["users_now"];
            this.UsersMax = (int)Row["users_max"];
            this.ModelName = (string)Row["model_name"];
            this.CCTs = (string)Row["public_ccts"];
            this.Score = (int)Row["score"];
            this.Tags = new List<string>();
            this.AllowPets = Decoders.ConvertEnumToBool(Row["allow_pets"].ToString());
            this.AllowPetsEating = Decoders.ConvertEnumToBool(Row["allow_pets_eat"].ToString());
            this.AllowWalkthrough = Decoders.ConvertEnumToBool(Row["allow_walkthrough"].ToString());
            this.AllowRightsOverride = Decoders.ConvertEnumToBool(Row["allow_rightsoverride"].ToString());
            this.Hidewall = Decoders.ConvertEnumToBool(Row["allow_hidewall"].ToString());
            this.Password = (string)Row["password"];
            this.Wallpaper = (string)Row["wallpaper"];
            this.Floor = (string)Row["floor"];
            this.Landscape = (string)Row["landscape"];
            //this.Event = null;

            Dictionary<int, int> IconItems = new Dictionary<int, int>();

            if (!string.IsNullOrEmpty(Row["icon_items"].ToString()))
            {
                foreach (string Bit in Row["icon_items"].ToString().Split('|'))
                {
                    if (string.IsNullOrEmpty(Bit))
                        continue;

                    string[] tBit = Bit.Replace('.', ',').Split(',');

                    int a = 0;
                    int b = 0;

                    int.TryParse(tBit[0], out a);
                    if (tBit.Length > 1)
                        int.TryParse(tBit[1], out b);

                    try
                    {
                        if (!IconItems.ContainsKey(a))
                            IconItems.Add(a, b);
                    }
                    catch (Exception Error)
                    {
                        Out.WriteLine(Error.Message, ConsoleColor.DarkRed, "   ", "Habbo.Rooms");
                    }
                }
            }

            //this.myIcon = new RoomIcon((int)Row["icon_bg"], (int)Row["icon_fg"], IconItems);

            foreach (string Tag in Row["tags"].ToString().Split(','))
            {
                this.Tags.Add(Tag);
            }

            //mModel = InfoEnv.GetGame().GetRoomManager().GetModel(ModelName, Id);
        }
    }
}
