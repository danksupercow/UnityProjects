using System;
using System.Net.Sockets;

public class Client
{
    private const int maxBufferSize = 4096;

    public int connectionID;
    public Player player;
    public string ip;
    public TcpClient socket;
    public NetworkStream myStream;
    private byte[] readBuff;

    public Client(TcpClient _socket, int _index, string _ip)
    {
        socket = _socket;
        connectionID = _index;
        ip = _ip;

        Start();
    }

    public Client()
    {
        socket = null;
    }

    public void Start()
    {
        socket.SendBufferSize = maxBufferSize;
        socket.ReceiveBufferSize = maxBufferSize;
        myStream = socket.GetStream();
        readBuff = new byte[maxBufferSize];
        myStream.BeginRead(readBuff, 0, socket.ReceiveBufferSize, OnReceiveData, null);
    }

    private void OnReceiveData(IAsyncResult ar)
    {
        try
        {
            int readbytes = myStream.EndRead(ar);
            if (readbytes <= 0)
            {
                Console.WriteLine("readBytes <= 0");
                CloseSocket();
                return;
            }
            byte[] newBytes = new byte[readbytes];
            Buffer.BlockCopy(readBuff, 0, newBytes, 0, readbytes);
            ServerHandleData.HandleData(connectionID, newBytes);
            myStream.BeginRead(readBuff, 0, socket.ReceiveBufferSize, OnReceiveData, null);
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            CloseSocket();
            return;
        }
    }

    private void CloseSocket()
    {
        //player.JustConnected = true;
        Console.WriteLine("Connection from " + ip + " has failed.");
        if(socket != null)
        {
            socket.Close();
            socket = null;
        }

        //Tell All Other Players That This Player Has Disconnected From The Server
        ServerTCP.SendPlayerLeft(connectionID);
        General.WritePlayersInfo();
    }
}
