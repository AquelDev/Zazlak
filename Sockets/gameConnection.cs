using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Zazlak.Sockets;
using System.Threading;
using Zazlak.Habbo;

using Zazlak.Habbo.Requests;

namespace Zazlak.Sockets
{
    public class gameConnection
    {
        #region Declares
        private Socket ClientSocket;
        private byte[] dataBuffer = new byte[1024];
        private int _ConnectionID;

        private User ConnectedUser;
        private RequestMessages Handler;

        private bool SocketClosed = false;
        private bool SocketReceivedPing = true;
        private AsyncCallback Callback;
        #endregion

        #region Constructor
        public gameConnection(Socket _ClientSocket, int _ConnectionID)
        {
            this.ConnectedUser = new User(this);
            this.Handler = new RequestMessages(_ConnectionID);
            Handler.User = this.ConnectedUser;
            Handler.LoadRequests(this.ConnectedUser);

            this.ClientSocket = _ClientSocket;
            this._ConnectionID = _ConnectionID;

            ConnectedUser.pingOK = true;
            Callback = new AsyncCallback(ReceivedData);

            WaitForData();
            Ping();

            connectionHelper.AddConnection(this, _ConnectionID);
        }
        #endregion

        #region Properities
        internal string connectionRemoteIP
        {
            get
            {
                return ClientSocket.RemoteEndPoint.ToString().Split(':')[0];
            }
        }

        internal RequestMessages RemoteHandler
        {
            get
            {
                return Handler;
            }
        }

        internal User Users
        {
            get
            {
                return ConnectedUser;
            }
        }

        internal bool IsActive
        {
            get
            {
                try
                {
                    return (bool)ClientSocket.Connected;
                }
                catch
                {
                    return false;
                }
            }
        }

        internal int ConnectionID
        {
            get
            {
                return this._ConnectionID;
            }
        }
        #endregion

        #region Methods
        private void WaitForData()
        {
            if (!SocketClosed)
            {
                try
                {
                    ClientSocket.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, Callback, null);
                }
                catch
                {
                    Close();
                }
            }
            else
                Close();
        }

        internal void Ping()
        {
            try
            {
                if (!ClientSocket.Connected)
                    Close();
            }
            catch
            {
                Close();
            }
        }

        private void ReceivedData(IAsyncResult iAr)
        {
            try
            {
                int bytesReceived = new int();

                try
                {
                    bytesReceived = ClientSocket.EndReceive(iAr);
                }
                catch
                {
                    Close();
                    return;
                }

                string ReceiveData = System.Text.Encoding.Default.GetString(dataBuffer, 0, bytesReceived);
                //if (ReceiveData.StartsWith("<policy-file-request/>"))
                if (ReceiveData.Contains("<policy-file-request/>"))
                {
                    string Data = "<?xml version=\"1.0\"?>" + "\r\n" + "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">" + "\r\n" + "<cross-domain-policy>" + "\r\n" + "   <allow-access-from domain=\"*\" to-ports=\"1-65535\" />" + "\r\n" + "</cross-domain-policy>\0";
                    byte[] dataBytes = System.Text.Encoding.Default.GetBytes(Data);
                    sendData(dataBytes);
                    Close();
                }
                else
                {
                    while (ReceiveData.Length > 0)
                    {
                        ClientMessage Message = new ClientMessage(ReceiveData);
                        if (ReceiveData.Length > 1)
                        {
                            Handler.ProcessPackets(ReceiveData.Substring(0, 4 + Message.Lenght()));
                            ReceiveData = ReceiveData.Remove(0, 4 + Message.Lenght());
                        }
                    }
                }
            }
            catch
            {
                Close();
            }
            finally
            {
                WaitForData();
            }
        }

        internal void Close()
        {
            if (!SocketClosed)
            {
                SocketClosed = true;
                try
                {
                    ClientSocket.Shutdown(SocketShutdown.Both);
                    ClientSocket.Close();
                    gameSocketServer.freeConnection(_ConnectionID);
                }
                catch { }
                ConnectedUser.Reset();
                connectionHelper.RemoveConnection(_ConnectionID);
                Crash();
            }
        }

        internal void sendData(byte[] Data)
        {
            try
            {
                ClientSocket.BeginSend(Data, 0, Data.Length, 0, new AsyncCallback(sentData), null);
            }
            catch
            {
                Close();
            }
        }

        private void sentData(IAsyncResult iAr)
        {
            try { ClientSocket.EndSend(iAr); }
            catch
            {
                Close();
            }
        }

        internal void Pinged()
        {
            this.SocketReceivedPing = true;
        }

        private void Crash()
        {
            ClientSocket = null;
            dataBuffer = null;
            _ConnectionID = 0;
            ConnectedUser = null;
            Handler = null;
            SocketClosed = true;
            SocketReceivedPing = false;
            Callback = null;
        }
        #endregion
    }
}
