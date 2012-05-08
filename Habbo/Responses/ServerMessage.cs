using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zazlak.Habbo;

namespace Zazlak.Habbo.Responses
{
    #region Add Normal Packets
    class ServerMessage //By Itachi & xdr
    {
        private List<byte> Message = new List<byte>();

        public ServerMessage(int Header)
        {
            AppendShort(Header);
        }

        public void AppendShort(int i)
        {
            Int16 s = (Int16)i;
            AppendBytes(BitConverter.GetBytes(s), true);
        }

        public void AppendInt32(object i)
        {
            try
            {
                int Int = Convert.ToInt32(i);
                AppendBytes(BitConverter.GetBytes(Int), true);
            }
            catch (Exception Error)
            {
                Out.WriteLine(Error.Message, ConsoleColor.DarkRed, "", "Habbo.Codification");
            }
        }

        public void AppendBoolean(bool b)
        {
            AppendBytes(new byte[] { (byte)(b ? 1 : 0) }, false);
        }

        public void AppendString(object s)
        {
            try
            {
                String String = Convert.ToString(s);

                AppendShort(String.Length);
                AppendBytes(Encoding.ASCII.GetBytes(String), false);
            }
            catch (Exception Error)
            {
                Out.WriteLine(Error.Message, ConsoleColor.DarkRed, "", "Habbo.Codification");
            }
        }

        public void AppendBreak()
        {
            AppendInt32(-1);
        }

        private void AppendBytes(byte[] b, bool Int)
        {
            if (Int)
            {
                for (int i = (b.Length - 1); i > -1; i--)
                    Message.Add(b[i]);
            }
            else
                Message.AddRange(b);
        }

        public byte[] ToBytes()
        {
            var SendMessage = new List<byte>();

            SendMessage.AddRange(BitConverter.GetBytes(Message.Count));
            SendMessage.Reverse();
            SendMessage.AddRange(Message);
            return SendMessage.ToArray();
        }

        public override string ToString()
        {
            return ToBytes().ToString();
        }
    }
    #endregion

    #region Add a lot of Packets
    public class BuildMessage
    {
        internal List<byte> AppendPackets = new List<byte>();

        internal void AppendPacket(ServerMessage Message)
        {
            AppendPackets.AddRange(Message.ToBytes());
        }

        internal void AppendBuilder(BuildMessage oB)
        {
            AppendPackets.AddRange(oB.ToBytes());
        }

        public byte[] ToBytes()
        {
            return AppendPackets.ToArray();
        }
    }
    #endregion
}

