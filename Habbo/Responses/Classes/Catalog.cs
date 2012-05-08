using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Zazlak.Habbo.Responses
{
    class HabboCatalog
    {
        internal User User;

        public HabboCatalog(User User)
        {
            this.User = User;
        }

        internal void InitCatalog()
        {
            #region Packet
            ServerMessage InitCatalog = new ServerMessage(Headers.CatalogInit);
            InitCatalog.AppendBoolean(true);
            InitCatalog.AppendInt32(0);
            InitCatalog.AppendInt32(0);
            InitCatalog.AppendInt32(-1);
            InitCatalog.AppendString("root");
            InitCatalog.AppendBoolean(false);
            InitCatalog.AppendBoolean(false);
            InitCatalog.AppendInt32(Cache.Catalog.Categories.Count);
            
            foreach (Cache.Catalog PageData in Cache.Catalog.Categories)
            {
                InitCatalog.AppendBoolean(PageData.OpenPage);
                InitCatalog.AppendInt32(PageData.IconColor);
                InitCatalog.AppendInt32(PageData.IconImage);
                InitCatalog.AppendInt32(PageData.Id);
                InitCatalog.AppendString("");
                InitCatalog.AppendString(PageData.Name);

                List<Cache.Catalog> PrivatePages = Cache.Catalog.GetPagesForId(PageData.Id);
                InitCatalog.AppendInt32(PrivatePages.Count);
                foreach (Cache.Catalog SubpageData in PrivatePages)
                {
                    InitCatalog.AppendBoolean(SubpageData.OpenPage);
                    InitCatalog.AppendInt32(SubpageData.IconColor);
                    InitCatalog.AppendInt32(SubpageData.IconImage);
                    InitCatalog.AppendInt32(SubpageData.Id);
                    InitCatalog.AppendString("");
                    InitCatalog.AppendString(SubpageData.Name);
                    InitCatalog.AppendInt32(0);
                }
            }
            InitCatalog.AppendBoolean(false);
            User.sendPacket(InitCatalog);
            #endregion
        }

        public void GetPages()
        {
            #region Packet
            BuildMessage BuildCatalog = new BuildMessage();

            int PageId = User.ActualClientMessage.GetNextInt();
            Cache.Catalog Ld = Cache.Catalog.Page[PageId];

            if (Ld.EnabledPage)
            {
                if (Ld.MinRank > Convert.ToInt32(User.HabboUser.UserRow["rank"]))
                {
                    return;
                }
                ServerMessage CatalogPage = new ServerMessage(Headers.SelectPages);
                CatalogPage.AppendInt32(PageId);
                switch (Ld.Extra)
                {
                    case "frontpage":
                        #region FrontPage
                        CatalogPage.AppendString("frontpage3");
                        CatalogPage.AppendInt32(3);
                        CatalogPage.AppendString("catalog_frontpage_headline_shop_DKCOMNLDE_02");
                        CatalogPage.AppendString("topstory_balloonmachine");
                        CatalogPage.AppendString("");
                        CatalogPage.AppendInt32(11);
                        CatalogPage.AppendString("THE RARE BALLOON MACHINE");
                        CatalogPage.AppendString("Celebrate New Year holding cheerful Balloons!");
                        CatalogPage.AppendString("The Last Xmas Rare âºâº");
                        CatalogPage.AppendString("How to get Habbo Credits");
                        CatalogPage.AppendString("You can get Habbo Credits via Prepaid Cards, Home Phone, Credit Card, Mobile, completing offers and more!" + Convert.ToChar(10).ToString() + "" + Convert.ToChar(10).ToString() + " To redeem your Habbo Credits, enter your voucher code below.");
                        CatalogPage.AppendString("Redeem a voucher code here:");
                        CatalogPage.AppendString("Snowflake Gifts");
                        CatalogPage.AppendString("#FEFEFE");
                        CatalogPage.AppendString("#FEFEFE");
                        CatalogPage.AppendString("Want all the options?  Click here!");
                        CatalogPage.AppendString("magic.credits");
                        CatalogPage.AppendInt32(0);
                        CatalogPage.AppendInt32(-1);
#endregion
                        break;

                    case "defaultpage":

                        CatalogPage.AppendString("default_3x3");
                        CatalogPage.AppendInt32(3);
                        CatalogPage.AppendString(Ld.HeadLine);
                        CatalogPage.AppendString(Ld.PageTeaser);
                        CatalogPage.AppendString(Ld.TextSpecial);
                        CatalogPage.AppendInt32(3);
                        CatalogPage.AppendString(Ld.Text);
                        CatalogPage.AppendString(Ld.TextDetails);
                        if (!Cache.Items.HaveDeals(Ld.Id))
                            CatalogPage.AppendString(Ld.TextTeaser);
                        CatalogPage.AppendInt32(0);
                        break;

                    case "VIPage":

                        if (Ld.Name == "VIP Club")
                        {
                            CatalogPage.AppendString("vip_buy");
                        }
                        else if (Ld.Name == "VIP Club as Gift")
                        {
                            CatalogPage.AppendString("vip_gift");
                        }
                        CatalogPage.AppendInt32(2);
                        CatalogPage.AppendString(Ld.HeadLine);
                        CatalogPage.AppendString(Ld.PageTeaser);
                        CatalogPage.AppendInt32(0);
                        CatalogPage.AppendInt32(0);
                        CatalogPage.AppendBreak();
                        BuildCatalog.AppendPacket(CatalogPage);
                        break;

                    case "VIPGifts":

                        CatalogPage.AppendString("club_gifts");
                        CatalogPage.AppendInt32(1);
                        CatalogPage.AppendString("ctlg_buy_vip_header");
                        CatalogPage.AppendInt32(1);
                        CatalogPage.AppendString("");
                        CatalogPage.AppendInt32(0);
                        break;

                    case "pets2":

                        CatalogPage.AppendString("pets");
                        CatalogPage.AppendInt32(2);
                        CatalogPage.AppendString(Ld.HeadLine);
                        CatalogPage.AppendString(Ld.PageTeaser);
                        CatalogPage.AppendInt32(4);
                        CatalogPage.AppendString(Ld.Text);
                        CatalogPage.AppendString(Ld.AnotherText);
                        CatalogPage.AppendString(Ld.TextDetails);
                        CatalogPage.AppendString(Ld.TextSpecial);
                        Console.WriteLine("Estamos aka");

                        break;

                    case "pet3":

                        CatalogPage.AppendString("petcustomization");
                        CatalogPage.AppendInt32(2);
                        CatalogPage.AppendString(Ld.HeadLine);
                        // CatalogPage.AppendString(Ld.PageTeaser);
                        CatalogPage.AppendString(Ld.TextSpecial);
                        CatalogPage.AppendInt32(3);
                        CatalogPage.AppendString(Ld.Text);
                        CatalogPage.AppendString(Ld.AnotherText);
                        CatalogPage.AppendString(Ld.TextDetails);

                        break;

                    case "lost_monkey":

                        CatalogPage.AppendString("monkey");
                        CatalogPage.AppendInt32(3);
                        CatalogPage.AppendString(Ld.HeadLine);
                        CatalogPage.AppendString(Ld.PageTeaser);
                        CatalogPage.AppendString(Ld.TextSpecial);
                        CatalogPage.AppendInt32(1);
                        CatalogPage.AppendString(Ld.Text);
                        CatalogPage.AppendString("");
                        CatalogPage.AppendString("");

                        break;

                    case "music":
                        #region Music
                        CatalogPage.AppendString("soundmachine");
                        CatalogPage.AppendInt32(2);
                        CatalogPage.AppendString(Ld.HeadLine);
                        CatalogPage.AppendString(Ld.PageTeaser);
                        CatalogPage.AppendInt32(2);
                        CatalogPage.AppendString(Ld.Text);
                        CatalogPage.AppendString(Ld.TextDetails);
#endregion
                        break;

                    case "guild":
                        #region guild
                        CatalogPage.AppendString("guild_frontpage");
                        CatalogPage.AppendInt32(2);
                        CatalogPage.AppendString(Ld.HeadLine);
                        CatalogPage.AppendString(Ld.PageTeaser);
                        CatalogPage.AppendInt32(3);
                        CatalogPage.AppendString("Habbo Groups are a great way to stay in touch with your friends and share your interests with others. Each Group has a homeroom that can be decorated by other Group members, members can also purchase exclusive Group Furni that can be customised with your Group colours!\n * Get together with people you get together with!\n * Co-op room decorating for group members\n * Show off your group badge!\n * Get some neat Furni in your group's colors!");
                        CatalogPage.AppendString("");
                        CatalogPage.AppendString("");
                        CatalogPage.AppendInt32(0);
#endregion
                        break;

                    case "spacepage":

                        CatalogPage.AppendString("spaces_new");
                        CatalogPage.AppendInt32(1);
                        CatalogPage.AppendString(Ld.HeadLine);
                        CatalogPage.AppendInt32(1);
                        CatalogPage.AppendString(Ld.Text);
                        break;
                }

                if (Ld.Extra == "VIPage")
                {
                    if (Ld.Name == "VIP Club")
                    {
                        #region Cuerpo del packet
                        ServerMessage VipButons = new ServerMessage(Headers.PageVipInit);
                        VipButons.AppendInt32(5);
                        VipButons.AppendInt32(10735); // Item Id??
                        VipButons.AppendString("HABBO_CLUB_VIP_1_DAY");
                        VipButons.AppendInt32(2);
                        VipButons.AppendBoolean(true);
                        VipButons.AppendInt32(0);
                        VipButons.AppendInt32(1); // Días
                        VipButons.AppendInt32(1); // Días
                        VipButons.AppendInt32(2011);
                        VipButons.AppendInt32(12);
                        VipButons.AppendInt32(22);
                        VipButons.AppendInt32(10734);
                        VipButons.AppendString("HABBO_CLUB_VIP_7_DAYS");
                        VipButons.AppendInt32(10);
                        VipButons.AppendBoolean(true);
                        VipButons.AppendInt32(0);
                        VipButons.AppendInt32(7); // Días
                        VipButons.AppendInt32(7); // Días
                        VipButons.AppendInt32(2011);
                        VipButons.AppendInt32(12);
                        VipButons.AppendInt32(0);
                        VipButons.AppendInt32(10733);
                        VipButons.AppendString("HABBO_CLUB_VIP_3_DAYS");
                        VipButons.AppendInt32(5);
                        VipButons.AppendBoolean(true);
                        VipButons.AppendInt32(0);
                        VipButons.AppendInt32(3); // Días
                        VipButons.AppendInt32(3); // Días
                        VipButons.AppendInt32(2011);
                        VipButons.AppendInt32(12);
                        VipButons.AppendInt32(24);
                        VipButons.AppendInt32(4898);
                        VipButons.AppendString("HABBO_CLUB_VIP_1_MONTH");
                        VipButons.AppendInt32(25); // COST
                        VipButons.AppendBoolean(true);
                        VipButons.AppendInt32(1);
                        VipButons.AppendInt32(0); // Días
                        VipButons.AppendInt32(0); // Días
                        VipButons.AppendInt32(2012);
                        VipButons.AppendInt32(1);
                        VipButons.AppendInt32(21);
                        VipButons.AppendInt32(19); //////////// ??
                        VipButons.AppendString("HABBO_CLUB_VIP_3_MONTHS");
                        VipButons.AppendInt32(60); // COST
                        VipButons.AppendBoolean(true);
                        VipButons.AppendInt32(3);
                        VipButons.AppendInt32(0); // Días
                        VipButons.AppendInt32(0); // Días
                        VipButons.AppendInt32(2012);
                        VipButons.AppendInt32(3);
                        VipButons.AppendInt32(23);
                        VipButons.AppendInt32(1);
                        BuildCatalog.AppendPacket(VipButons);
                        #endregion
                    }
                    else if (Ld.Name == "VIP Club as Gift")
                    {
                        #region Cuerpo del packet
                        ServerMessage VipButons = new ServerMessage(Headers.PageVipInit);
                        VipButons.AppendInt32(5);
                        VipButons.AppendInt32(10735); // Item Id??
                        VipButons.AppendString("HABBO_CLUB_VIP_1_DAY");
                        VipButons.AppendInt32(2);
                        VipButons.AppendBoolean(true);
                        VipButons.AppendInt32(0);
                        VipButons.AppendInt32(1); // Días
                        VipButons.AppendInt32(1); // Días
                        VipButons.AppendInt32(2011);
                        VipButons.AppendInt32(12);
                        VipButons.AppendInt32(22);
                        VipButons.AppendInt32(10734);
                        VipButons.AppendString("HABBO_CLUB_VIP_7_DAYS");
                        VipButons.AppendInt32(10);
                        VipButons.AppendBoolean(true);
                        VipButons.AppendInt32(0);
                        VipButons.AppendInt32(7); // Días
                        VipButons.AppendInt32(7); // Días
                        VipButons.AppendInt32(2011);
                        VipButons.AppendInt32(12);
                        VipButons.AppendInt32(0);
                        VipButons.AppendInt32(10733);
                        VipButons.AppendString("HABBO_CLUB_VIP_3_DAYS");
                        VipButons.AppendInt32(5);
                        VipButons.AppendBoolean(true);
                        VipButons.AppendInt32(0);
                        VipButons.AppendInt32(3); // Días
                        VipButons.AppendInt32(3); // Días
                        VipButons.AppendInt32(2011);
                        VipButons.AppendInt32(12);
                        VipButons.AppendInt32(24);
                        VipButons.AppendInt32(4898);
                        VipButons.AppendString("HABBO_CLUB_VIP_1_MONTH");
                        VipButons.AppendInt32(25); // COST
                        VipButons.AppendBoolean(true);
                        VipButons.AppendInt32(1);
                        VipButons.AppendInt32(0); // Días
                        VipButons.AppendInt32(0); // Días
                        VipButons.AppendInt32(2012);
                        VipButons.AppendInt32(1);
                        VipButons.AppendInt32(21);
                        VipButons.AppendInt32(19); //////////// ??
                        VipButons.AppendString("HABBO_CLUB_VIP_3_MONTHS");
                        VipButons.AppendInt32(60); // COST
                        VipButons.AppendBoolean(true);
                        VipButons.AppendInt32(3);
                        VipButons.AppendInt32(0); // Días
                        VipButons.AppendInt32(0); // Días
                        VipButons.AppendInt32(2012);
                        VipButons.AppendInt32(3);
                        VipButons.AppendInt32(23);
                        VipButons.AppendInt32(1);
                        BuildCatalog.AppendPacket(VipButons);
                        #endregion
                    }
                }
                else
                {
                    List<Cache.Items> ActualItems = Cache.Items.GetItemsForPageId(Ld.Id);
                    CatalogPage.AppendInt32(ActualItems.Count);

                    foreach (Cache.Items eItem in ActualItems)
                    {
                        this.SerializeCatalogItem(CatalogPage, Ld, eItem);
                    }
                }
                if (Ld.Extra != "VIPage" || Ld.Extra != "VIPGifts")
                {
                    CatalogPage.AppendBreak();
                    BuildCatalog.AppendPacket(CatalogPage);
                }

                User.sendPacket(BuildCatalog);
            }
            #endregion
        }

        internal void SerializeCatalogItem(ServerMessage CatalogPage, Cache.Catalog CPages, Cache.Items eItem)
        {
            #region CatalogItem
            if (eItem.Name.Contains("wallpaper") || eItem.Name.Contains("floor") || eItem.Name.Contains("landscape"))
            {
                try
                {
                    string[] WallPaperPart = eItem.Name.Split('_');
                    CatalogPage.AppendInt32(eItem.Id); // Item Id
                    CatalogPage.AppendString(eItem.Name); // Item Name
                    CatalogPage.AppendInt32(eItem.Cost_Credits); // Cost (Credits)
                    CatalogPage.AppendInt32(eItem.Cost_Pixels); // Cost (Pixels, SnowFlakes or maybe Hearts)
                    CatalogPage.AppendInt32(CPages.PageQuest); // Page Type; 0 = nothing, 1 = Snowflakes, 2 = hearts, 3 = ?
                    CatalogPage.AppendBoolean(true); // ??
                    CatalogPage.AppendInt32(eItem.ItemIds.Count); // Count of Furnis of the deal
                    foreach (int FurniId in eItem.ItemIds)
                    {
                        Cache.FurniTure fItem = Cache.FurniTure.RegisteredFurnis[FurniId];
                        CatalogPage.AppendString(fItem.Type); // Special type of furni?
                        CatalogPage.AppendInt32(fItem.SpriteId); // SpriteID
                        CatalogPage.AppendString(WallPaperPart[2]); // Separe
                        CatalogPage.AppendInt32(eItem.Amount);
                        CatalogPage.AppendBreak(); // Separe
                    }
                    CatalogPage.AppendInt32(eItem.IsClub); // Is HC Button?
                }
                catch { return; }
            }
            else
            {
                try
                {
                    CatalogPage.AppendInt32(eItem.Id); // Item Id
                    CatalogPage.AppendString(eItem.Name); // Item Name
                    CatalogPage.AppendInt32(eItem.Cost_Credits); // Cost (Credits)
                    CatalogPage.AppendInt32(eItem.Cost_Pixels); // Cost (Pixels, SnowFlakes or maybe Hearts)
                    CatalogPage.AppendInt32(CPages.PageQuest); // Page Type; 0 = nothing, 1 = Snowflakes, 2 = hearts, 3 = ?
                    CatalogPage.AppendBoolean(true); // ??
                    CatalogPage.AppendInt32(eItem.ItemIds.Count); // Count of Furnis of the deal
                    foreach (int FurniId in eItem.ItemIds)
                    {
                        Cache.FurniTure fItem = Cache.FurniTure.RegisteredFurnis[FurniId];
                        CatalogPage.AppendString(fItem.Type); // Type ("i","s",etc)
                        CatalogPage.AppendInt32(fItem.SpriteId); // SpriteID
                        CatalogPage.AppendString(eItem.ExtraInformation); // Separe Char (like a AppendString(""))
                        if (eItem.ExtraAmounts.ContainsKey(FurniId))
                            CatalogPage.AppendInt32(eItem.ExtraAmounts[FurniId]); // Amount
                        else
                            CatalogPage.AppendInt32(eItem.Amount); // Amount
                        CatalogPage.AppendBreak(); // Separe
                    }
                    CatalogPage.AppendInt32(eItem.IsClub); // Is HC(1)/VIP(2) Item?
                }
                catch { return; }
            }
            #endregion
        }
    }
}
