using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

    public static NetworkManager instance;
    private ClientTCP clientTCP = new ClientTCP();
    [SerializeField]private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    private Dictionary<int, GameObject> playerList = new Dictionary<int, GameObject>();
    private static List<ServerData> playerServers = new List<ServerData>();
    public static int connectionID;

    public static TcpClient Socket { get { return instance.clientTCP.playerSocket; } }
    public static List<ServerData> PlayerServers { get { return playerServers; } }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        UnityThread.initUnityThread();
        instance = this;

        if(General.ReadPlayerData() != null)
        {
            playerServers = General.ReadPlayerData();
        }
    }

    private void LateUpdate()
    {
        return;

        

        if(clientTCP.playerSocket.Connected == false)
        {
            Debug.LogError("Connection To Server Was Lost!");
            clientTCP.playerSocket.Close();
        }
    }

    private void OnApplicationQuit()
    {
        clientTCP.Disconnect();
    }

    // Use this for initialization
    void Start () {
        clientTCP.Connect();
	}
	
    public void InstantiatePlayer(int index)
    {
        GameObject temp = Instantiate(playerPrefab);
        temp.name = "Player: " + index;
        temp.transform.position = spawnPoint.position;
        PlayerController ply = temp.GetComponent<PlayerController>();
        ViewController vc = ply.GetComponent<ViewController>();
        vc.connectionID = index;
        Stats s = temp.GetComponent<Stats>();
        s.Init();

        if (index != connectionID)
        {
            Destroy(ply.transform.Find("PlayerCamera").gameObject);
            Destroy(ply);
        }
        playerList.Add(index, temp);
    }

    public static GameObject GetPlayerObjectFromID(int id)
    {
        return instance.playerList[id];
    }

    public static void RegisterServer(string ip, int port)
    {
        if(playerServers.Count == 0)
        {
            playerServers.Add(new ServerData(ip, port));
            Debug.Log("Added Server Tp Empty List");
            General.WriteToPlayerDat();
            return;
        }

        for (int i = 0; i < playerServers.Count; i++)
        {
            if(playerServers[i].ServerIP == ip && playerServers[i].ServerPort == port)
            {
                General.WriteToPlayerDat();
                return;
            }
        }

        playerServers.Add(new ServerData(ip, port));
        Debug.Log("");
        General.WriteToPlayerDat();
    }

    public static string FetchServerDataUID(string ip, int port)
    {
        for (int i = 0; i < playerServers.Count; i++)
        {
            if (playerServers[i].ServerIP == ip && playerServers[i].ServerPort == port)
            {
                return playerServers[i].UID;
            }
        }

        return string.Empty;
    }

}
