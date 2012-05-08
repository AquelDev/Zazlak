using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Zazlak.Sockets
{

    public class TCPServer
    {

        public static void startMUS(int port)
        {
            IPAddress ipAddress = IPAddress.Any;
            TcpListener listener = new TcpListener(ipAddress, port);
            listener.Start();
            while (true)
            {
                Socket s = listener.AcceptSocket();
                byte[] b = new byte[65535];
                int k = s.Receive(b);
                Console.Write("SSO Ticket: ");
                for (int i = 0; i < k; i++)
                    Console.Write(Convert.ToChar(b[i]));
                Console.WriteLine();
                ASCIIEncoding enc = new ASCIIEncoding();
                s.Close();
            }
        }
    }
}
