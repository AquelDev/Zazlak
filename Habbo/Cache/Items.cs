using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Zazlak.Storage;

namespace Zazlak.Habbo.Cache
{
    class Items
    {
        internal int Id;
        internal int PageId;
        internal string Name;
        internal int Cost_Credits;
        internal int Cost_Pixels;
        internal int Cost_AnythingQuestPoint;
        internal int FinalPixels_orQuest;
        internal int Amount;
        internal string FurniId;
        //internal int SpriteId;
        internal List<int> ItemIds;
        //internal string Type;
        internal int IsClub;
        internal string extraAm;
        internal Dictionary<int, int> ExtraAmounts;
        //internal ItemsData BaseItem;
        internal Catalog PageData()
        {
            if (this.PageId < 0)
                return null;
            else
                return Catalog.Page[this.PageId];
        }
        internal string ExtraInformation;
        public static List<Items> CatalogItems;
        public static Dictionary<int, Items> CatalogItemsForId;


        public static void InitCatalogItems()
        {
            CatalogItems = new List<Items>();
            CatalogItemsForId = new Dictionary<int, Items>();

            MySQL RowItems = new MySQL(Init.ServerMySQL);
            RowItems.Query("SELECT * FROM catalog_items ORDER BY id ASC");

            foreach (DataRow Row in RowItems.Fetch_Array())
            {
                Items I = new Items();
                I.Id = Convert.ToInt32(Row["id"]);
                I.PageId = Convert.ToInt32(Row["pageid"]);
                I.Name = Convert.ToString(Row["name"]);
                I.Cost_Credits = Convert.ToInt32(Row["cost_credits"]);
                I.Cost_Pixels = Convert.ToInt32(Row["cost_pixels"]);
                I.Cost_AnythingQuestPoint = Convert.ToInt32(Row["cost_quest"]);
                if (Convert.ToInt32(Row["cost_quest"]) > 0)
                {
                    I.FinalPixels_orQuest = Convert.ToInt32(Row["cost_quest"]);
                }
                else
                {
                    I.FinalPixels_orQuest = Convert.ToInt32(Row["cost_pixels"]);
                }
                I.Amount = Convert.ToInt32(Row["amount"]);
                I.extraAm = Convert.ToString(Row["extraamounts"]);
                I.ExtraAmounts = new Dictionary<int, int>();
                if (I.extraAm.Contains(";"))
                {
                    string[] separe = I.extraAm.Split(';');
                    foreach (string s in separe)
                    {
                        if (s == "")
                            continue;

                        string[] s2 = s.Split(',');

                        if (I.ExtraAmounts.ContainsKey(int.Parse(s2[0])))
                            continue;

                        I.ExtraAmounts.Add(int.Parse(s2[0]), int.Parse(s2[1]));
                    }
                }
                I.FurniId = Convert.ToString(Row["furni_id"]);
                I.ItemIds = new List<int>();
                if (I.FurniId.Contains(";"))
                {
                    string[] separe = I.FurniId.Split(';');
                    foreach (string s in separe)
                    {
                        if (I.ItemIds.Contains(int.Parse(s)))
                            continue;

                        if (s == "")
                            continue;

                        I.ItemIds.Add(int.Parse(s));
                    }
                }
                else
                    I.ItemIds.Add(int.Parse(I.FurniId));
                I.IsClub = int.Parse(Row["is_club"].ToString());
                I.ExtraInformation = Convert.ToString(Row["extrainformation"]);
                CatalogItems.Add(I);
                CatalogItemsForId.Add(I.Id, I);
            }

            Out.WritePlain("[Zazlak] > Catalog Items loaded!", ConsoleColor.Green);
        }

        public static bool HaveDeals(int PageId)
        {
            List<Items> C = GetItemsForPageId(PageId);

            int i = 0;
            int e = 0;
            foreach (Items c in C)
            {
                if (c.ItemIds.Count > 1)
                    i++;
                else
                    e++;
            }

            if (i > 0 && e == 0)
                return true;
            else
                return false;
        }

        public static List<Items> GetItemsForPageId(int PageId)
        {
            List<Items> D = new List<Items>();
            foreach (Items Data in CatalogItems)
            {
                if (Data.PageId == PageId)
                    D.Add(Data);
            }
            return D;
        }
    }
}
