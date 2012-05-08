using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Zazlak.Storage;

namespace Zazlak.Habbo.Cache
{
    class FurniTure
    {
        internal int Id;
        internal int Width;
        internal int Length;
        internal double Height;
        internal int SpriteId;
        internal string Name;
        internal string Type;
        internal string FurniInteractor;
        internal bool CanGift;
        internal bool CanRecycle;
        internal bool CanTrade;
        internal bool CanStack;
        internal bool CanSell;
        internal bool CanWalk;
        internal bool CanSit;
        public static List<FurniTure> Furnis;
        public static Dictionary<int, FurniTure> RegisteredFurnis;

        public static void InitItems()
        {
            Furnis = new List<FurniTure>();
            RegisteredFurnis = new Dictionary<int, FurniTure>();

            MySQL Query = new MySQL(Init.ServerMySQL);
            Query.Query("SELECT * FROM furniture");

            foreach (DataRow Row in Query.Fetch_Array())
            {
                FurniTure I = new FurniTure();
                I.Id = Convert.ToInt32(Row["id"]);
                I.Type = Row["type"].ToString();
                I.SpriteId = Convert.ToInt32(Row["sprite_id"]);
                I.Width = Convert.ToInt32(Row["width"]);
                I.Length = Convert.ToInt32(Row["length"]);
                I.Name = Row["name"].ToString();
                I.Type = Convert.ToString(Row["type"]);
                I.FurniInteractor = Convert.ToString(Row["furni_type"]);
                I.CanGift = Decoders.ConvertEnumToBool(Row["can_gift"].ToString());
                I.CanRecycle = Decoders.ConvertEnumToBool(Row["can_recycle"].ToString());
                I.CanTrade = Decoders.ConvertEnumToBool(Row["can_trade"].ToString());
                I.CanStack = Decoders.ConvertEnumToBool(Row["can_stack"].ToString());
                I.CanSell = Decoders.ConvertEnumToBool(Row["can_sell"].ToString());
                I.CanWalk = Decoders.ConvertEnumToBool(Row["can_walk"].ToString());
                I.CanSit = Decoders.ConvertEnumToBool(Row["can_sit"].ToString());
                I.Height = Convert.ToDouble(Row["height"]);
                Furnis.Add(I);
                RegisteredFurnis.Add(I.Id, I);
            }
        }

        public static int GetLastId()
        {
            int i = 0;
            foreach (FurniTure U in Furnis)
            {
                if (U.Id > i)
                    i = U.Id;
                else
                    continue;
            }

            return i;
        }
    }
}
