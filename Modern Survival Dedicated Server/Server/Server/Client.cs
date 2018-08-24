using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Client
    {
        private const int maxBufferSize = 4096;

        public int connectionID;
        public string ip;
        public TcpClient socket;
        public NetworkStream stream;
        private byte[] readBuff;

        public Client(TcpClient _socket, int _connectionID, string _ip)
        {
            socket = _socket;
            connectionID = _connectionID;
            ip = _ip;

            Init();
        }

        public void Init()
        {
            socket.SendBufferSize = maxBufferSize;
            socket.ReceiveBufferSize = maxBufferSize;
            stream = socket.GetStream();
            readBuff = new byte[maxBufferSize];
            stream.BeginRead(readBuff, 0, socket.ReceiveBufferSize, ReceivedData, null);
        }

        private void ReceivedData(IAsyncResult result)
        {
            try
            {
                int readbytes = stream.EndRead(result);
                if(readbytes <= 0)
                {
                    Close("Read Bytes Was <= 0");
                    return;
                }
                byte[] newBytes = new byte[readbytes];
                Buffer.BlockCopy(readBuff, 0, newBytes, 0, readbytes);
                //HANDLE DATA HERE
                stream.BeginRead(readBuff, 0, socket.ReceiveBufferSize, ReceivedData, null);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Close("Received Data Couldn't Be Processed");
            }
        }

        public void Close(string reason)
        {
            //Tell server to tell all clients this client left!
            if(socket != null)
            {
                socket.Close();
                socket = null;
            }

            Debug.LogUpdate("Player [" + connectionID + "] Was Disconnected: " + reason);
        }
    }
}
