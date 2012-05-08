using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Zazlak.Storage;

namespace Zazlak.Habbo.Cache
{
    class Catalog
    {
        internal int Id;
        internal int CategoryId;
        internal string Name;
        internal bool OpenPage;
        internal bool ClubPage;
        internal int MinRank;
        internal int IconImage;
        internal int IconColor;
        internal string Extra;
        internal string HeadLine;
        internal bool EnabledPage;
        internal string Text;
        internal string AnotherText;
        internal string TextDetails;
        internal string TextSpecial;
        internal string TextTeaser;
        internal string PageTeaser;
        internal int PageQuest;
        public static List<Catalog> Categories;
        public static List<Catalog> Pages;
        public static Dictionary<int, Catalog> Page;

        public static void InitCatalogPages()
        {
            try
            {
                Categories = new List<Catalog>();
                Pages = new List<Catalog>();
                Page = new Dictionary<int, Catalog>();

                MySQL MySql = new MySQL(Init.ServerMySQL);
                MySql.Query("SELECT * FROM catalog_pages ORDER BY order_num ASC");

                foreach (DataRow Row in MySql.Fetch_Array())
                {
                    Catalog C = new Catalog();
                    C.Id = Convert.ToInt32(Row["id"]);
                    C.CategoryId = Convert.ToInt32(Row["categoryid"]);
                    C.Name = Convert.ToString(Row["name"]);
                    C.OpenPage = Decoders.ConvertEnumToBool(Row["page_open"]);
                    C.EnabledPage = Decoders.ConvertEnumToBool(Row["page_enabled"].ToString());
                    C.ClubPage = Decoders.ConvertEnumToBool(Row["club_page"].ToString());
                    C.MinRank = Convert.ToInt32(Row["min_rank"]);
                    C.IconColor = Convert.ToInt32(Row["icon_color"]);
                    C.IconImage = Convert.ToInt32(Row["icon_image"]);
                    C.Extra = Row["page_extra"].ToString();
                    C.HeadLine = Convert.ToString(Row["page_headline"]);
                    C.PageTeaser = Convert.ToString(Row["page_teaser"]);
                    C.Text = Convert.ToString(Row["page_text"]);
                    C.AnotherText = Convert.ToString(Row["page_othertext"]);
                    C.TextDetails = Convert.ToString(Row["page_text_details"]);
                    C.TextSpecial = Convert.ToString(Row["page_special"]);
                    C.TextTeaser = Convert.ToString(Row["page_text_teaser"]);
                    C.PageQuest = int.Parse(Row["questid"].ToString());
                    if (C.CategoryId > -1)
                        Pages.Add(C);
                    else
                        Categories.Add(C);
                    Page.Add(C.Id, C);
                }

                Out.WritePlain("[Zazlak] > Catalog loaded!", ConsoleColor.Green);
            }
            catch (Exception Error)
            {
                Out.WritePlain("[Zazlak] > " + Error.Message, ConsoleColor.DarkRed);
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        public static List<Catalog> GetPagesForId(int Id)
        {
            List<Catalog> L = new List<Catalog>();
            foreach (Catalog Data in Pages)
            {
                if (Data.CategoryId == Id)
                    L.Add(Data);
            }
            return L;
        }
    }
}
