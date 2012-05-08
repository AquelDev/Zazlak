using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zazlak.Habbo;

namespace Zazlak.Habbo
{
    public class ClientMessage
    {
        public String oString;
        private String oData;
        public ClientMessage(string Data)
        {
            oString = Data;
            oData = oString.Substring(6);
        }

        public int Lenght()
        {
            return Decoders.DecodeBit8(oString.Substring(2, 2));
        }

        public int Header()
        {
            return Decoders.DecodeBit8(oString.Substring(4, 2));
        }

        public int GetNextInt()
        {
            try
            {
                int result = Decoders.DecodeBit24(oData.Substring(0, 4));
                oData = oData.Substring(4);

                return result;
            }
            catch
            {
                return 1;
            }
        }

        public String GetNextString()
        {
            int len = Decoders.DecodeBit8(oData.Substring(0, 2));
            oData = oData.Substring(2);

            String Result = oData.Substring(0, len);
            oData = oData.Substring(len);

            return Result;
        }
    }
}
