using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

using System.Collections;
using Zazlak.Sockets;

namespace Zazlak.Sockets
{
    public static class gameSocketServer
    {
        #region Declares
        private static Socket socketHandler;
        private static int _Port = 80;
        private static int _maxConnections = 3306;
        private static int _acceptedConnections;
        private static HashSet<int> _activeConnections;
        private static Hashtable connections = new Hashtable(2);
        private static bool AcceptConnections = true;
        public static string[] _activeIPs;
        private static Object connObj = new Object();

        #endregion

        #region Methods

        private static void connectionRequest(IAsyncResult iAr)
        {
            try
            {
                int connectionID = 0;
                for (int i = 1; i < _maxConnections; i++)
                {
                    if (_activeConnections.Contains(i) == false)
                    {
                        connectionID = i;
                        break;
                    }
                }
            }
            catch { }
            socketHandler.BeginAccept(new AsyncCallback(connectionRequest), socketHandler);
        }

        internal static void SetupSocket(Int32 Port, Int32 MaxCon)
        {
            _Port = Port;
            _maxConnections = MaxCon;

            connectionHelper.Init();
            AcceptConnections = true;
            if (_activeConnections != null)
                _activeConnections.Clear();
            _activeConnections = new HashSet<int>();

            StartListening();
        }

        internal static void Destroy()
        {
            AcceptConnections = false;
            HashSet<int> Data = _activeConnections;

            foreach (int ConnectionID in Data)
            {
                freeConnection(ConnectionID);
            }
        }

        internal static void StartListening()
        {
            bool SomethingWentWrong = false;
            try
            {
                if (_Port == 0 || _maxConnections == 0)
                    throw new NullReferenceException();
                else
                {
                    socketHandler = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPEndPoint Endpoint = new IPEndPoint(IPAddress.Any, _Port);
                    socketHandler.Bind(Endpoint);
                    socketHandler.Listen(1000);

                    while (!AcceptConnections)
                    {
                        socketHandler.BeginAccept(new AsyncCallback(InncommingDataRequest), socketHandler);
                    }
                }
            }
            catch (Exception Error)
            {
                Out.WritePlain("[Zazlak] > "+Error.Message, ConsoleColor.DarkRed);
                Console.ReadKey();
                Environment.Exit(0);
            }
            finally
            {
                if (!SomethingWentWrong)
                {
                    socketHandler.BeginAccept(new AsyncCallback(InncommingDataRequest), socketHandler);
                    Out.WritePlain("[Zazlak] > Sockets are listening on *:"+_Port+"", ConsoleColor.Green);
                }
            }
        }

        internal static void InncommingDataRequest(IAsyncResult iAr)
        {
            try
            {
                Socket ReplyFromComputer = ((Socket)iAr.AsyncState).EndAccept(iAr);
                string IP = ReplyFromComputer.RemoteEndPoint.ToString().Split(':')[0];
                int amount = GetConnectionAmount(IP);
                if (acceptedConnections >= maxConnections || AcceptConnections == false)
                {
                    ReplyFromComputer.Shutdown(SocketShutdown.Both);
                    ReplyFromComputer.Close();
                }
                else if (amount > 25)
                {
                    ReplyFromComputer.Shutdown(SocketShutdown.Both);
                    ReplyFromComputer.Close();
                }
                else
                {
                    int ConnectionRequestID = NewConnectionRequestID();
                    if (ConnectionRequestID > 0)
                    {
                        //Console.Title = "Azure Emulator | Sessions ["+ConnectionRequestID+"]";
                        Out.WriteLine("Conexión entrante [" + ConnectionRequestID + "] de " + IP, ConsoleColor.Blue);
                        AddConnection(IP);
                        new gameConnection(ReplyFromComputer, ConnectionRequestID);
                    }
                }
            }
            catch { }
            socketHandler.BeginAccept(new AsyncCallback(InncommingDataRequest), socketHandler);
        }

        internal static int NewConnectionRequestID()
        {
            _acceptedConnections++;
            _activeConnections.Add(_acceptedConnections);
            return _acceptedConnections;
        }

        internal static void freeConnection(int connectionID)
        {
            if (_activeConnections.Contains(connectionID))
            {
                _activeConnections.Remove(connectionID);
                Out.WriteLine("Conexión saliente [" + connectionID + "] liberada.", ConsoleColor.DarkBlue);
                _acceptedConnections--;
            }
        }

        internal static void AddConnection(string IP)
        {

            int Amount = 0;
            if (connections.ContainsKey(IP) == true)
            {
                Amount = (int)connections[IP];
                connections.Remove(IP);
            }
            else
            {
                Amount = 0;
            }

            int newamount = Amount + 1;
            connections.Add(IP, newamount);
            //Thread.Sleep(50);
        }

        internal static void RemoveConnection(string IP)
        {
            int IpConnected = 0;
            if (connections.ContainsKey(IP) == true)
            {
                IpConnected = (int)connections[IP];
            }
            else
            {
                connections.Remove(IP);
            }


            if (IpConnected > 1) //More than one connected
            {
                int NewAmount = IpConnected - 1;
                connections.Remove(IP);
                connections.Add(IP, NewAmount);
            }
            else //Only one connected
            {
                connections.Remove(IP);
            }
        }


        #endregion

        #region Properities
        internal static int GetConnectionAmount(string IP)
        {

            if (connections.ContainsKey(IP) == true)
            {
                return ((int)connections[IP]);
            }
            else
            {
                return 0;
            }
        }

        internal static int maxConnections
        {
            set
            {
                _maxConnections = value;
            }
            get
            {
                return _maxConnections;
            }
        }

        internal static int acceptedConnections
        {
            get {
                return _acceptedConnections;
            }
        }
        #endregion
    }
}
