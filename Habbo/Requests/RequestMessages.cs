using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zazlak.Habbo;


namespace Zazlak.Habbo.Requests
{
    partial class RequestMessages
    {
        internal delegate void RequestPackets();
        internal RequestPackets[] RequestPacket;
        private int ConnectionId;

        internal User User;

        internal RequestMessages(int ActualGame)
        {
            this.ConnectionId = ActualGame;
            RequestPacket = new RequestPackets[99999]; // Get All
        }

        internal void ProcessPackets(string Packet)
        {
            try
            {
                ClientMessage Mess = new ClientMessage(Packet);
                Out.Write("[" + Mess.Header() + "] » ", ConsoleColor.Gray, "");

                if (RequestPacket[Mess.Header()] == null)
                {
                    Out.Write("No Registrado", ConsoleColor.DarkRed, "");
                    Out.WriteBlank();
                }
                else
                {
                    Out.Write("Registrado", ConsoleColor.DarkGreen, "");
                    Out.WriteBlank();
                    User.ActualClientMessage = Mess;
                    User.ActualPacket = Packet;
                    RequestPacket[Mess.Header()].Invoke();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Azure ha encontrado un error en el código: " + e.ToString());
            }
        }
    }
}
