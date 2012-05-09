using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zazlak.Habbo;

namespace Zazlak.Habbo.Requests
{
    partial class RequestMessages
    {
        internal void LoadRequests(User User)
        {
            //UserData
            RequestPacket[4000] = new RequestPackets(User.HabboUser.Login);
            RequestPacket[8481] = new RequestPackets(User.HabboUser.sendPacket);
            RequestPacket[3670] = new RequestPackets(User.HabboUser.HomeRoom);
            RequestPacket[2806] = new RequestPackets(User.HabboUser.UpdateLook); // 30/12/2011
            RequestPacket[2135] = new RequestPackets(User.HabboUser.Sings); // 30/12/2011
            RequestPacket[1339] = new RequestPackets(User.HabboUser.Dance); // 30/12/2011
            RequestPacket[2812] = new RequestPackets(User.HabboUser.Wave); // 30/12/2011
            RequestPacket[372] = new RequestPackets(User.HabboUser.Sit); // 30/12/2011
            RequestPacket[2812] = new RequestPackets(User.HabboUser.Idle); // 30/12/2011
            RequestPacket[2301] = new RequestPackets(User.HabboUser.ChangeMotto); // 30/12/2011
            RequestPacket[9506] = new RequestPackets(User.HabboUser.UserProfile);
            RequestPacket[3108] = new RequestPackets(User.HabboUser.Chatting);
            RequestPacket[11037] = new RequestPackets(User.HabboUser.Stream);
            RequestPacket[167] = new RequestPackets(User.HabboUser.Ping);
            

            //Catalog
            RequestPacket[3903] = new RequestPackets(User.HabboCatalog.InitCatalog); // 30/12/2011
            RequestPacket[1640] = new RequestPackets(User.HabboCatalog.GetPages); // 30/12/2011
            
            //Navigator
            RequestPacket[3760] = new RequestPackets(User.HabboNavigator.MyRooms); // 30/12/2011
            RequestPacket[3435] = new RequestPackets(User.HabboNavigator.Search); // 30/12/2011

            //Rooms
            RequestPacket[1373] = new RequestPackets(User.HabboRooms.LoadRoom); // 30/12/2011
            RequestPacket[3918] = new RequestPackets(User.HabboRooms.LoadModel); // 30/12/2011
            RequestPacket[2232] = new RequestPackets(User.HabboRooms.ThirdRequest); // 30/12/2011
            RequestPacket[697] = new RequestPackets(User.HabboRooms.FourthRequest); // 30/12/2011
        }
    }
}
