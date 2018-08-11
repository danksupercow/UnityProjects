using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

    public static NetworkManager instance;
    private ClientTCP clientTCP = new ClientTCP();
    [SerializeField]private GameObject playerPrefab;
    [SerializeField] private Camera menuCamera;
    [SerializeField] private Transform spawnPoint;

    /* ### Lists... ### */
    private Dictionary<int, GameObject> playerList = new Dictionary<int, GameObject>();
    private static List<ServerData> playerServers = new List<ServerData>();
    [SerializeField]
    private RegisteredPrefab[] registeredPrefabs;

    /* ### ID Shit... ### */
    public static int connectionID;

    /* ### Properties... ### */
    public static TcpClient Socket { get { return instance.clientTCP.playerSocket; } }
    public static List<ServerData> PlayerServers { get { return playerServers; } }
    public static ClientTCP Client { get { return instance.clientTCP; } }
    public static Vector3 LocalPlayerPosition { get { return GetPlayerObjectFromID(connectionID).transform.position; } }

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

    private void OnApplicationQuit()
    {
        clientTCP.Disconnect();
    }

    // Use this for initialization
    void Start () {
        //clientTCP.Connect();
	}
	
    public GameObject InstantiatePlayer(int index)
    {
        GameObject temp = Instantiate(playerPrefab);

        try
        {
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
                Destroy(vc);
            }
            else
            {
                menuCamera.enabled = false;
            }
            playerList.Add(index, temp);

            Console.Log("Created Player: " + index + " successfully.");
        }
        catch (System.Exception e)
        {
            Console.LogError(e);
        }
        return temp;
    }

    public static GameObject GetPlayerObjectFromID(int id)
    {
        if (instance.playerList.Count < (id + 1))
            return null;

        if (instance.playerList.Count == 0)
            return null;

        return instance.playerList[id];
    }

    public static void RegisterServer(string ip, int port)
    {
        if(playerServers.Count == 0)
        {
            playerServers.Add(new ServerData(ip, port));
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
        ServerData sd = new ServerData(ip, port);
        playerServers.Add(sd);

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

    public static ServerData FetchServerData(string ip, int port)
    {
        for (int i = 0; i < playerServers.Count; i++)
        {
            if(playerServers[i].ServerIP == ip && playerServers[i].ServerPort == port)
            {
                return playerServers[i];
            }
        }

        return null;
    }

    public static Transform SpawnRegisteredPrefab(string slug)
    {
        for (int i = 0; i < instance.registeredPrefabs.Length; i++)
        {
            if(instance.registeredPrefabs[i].slug == slug)
            {
                Transform t = Instantiate(instance.registeredPrefabs[i].prefab).transform;
                return t;
            }
        }

        Debug.LogError("Failed to find prefab with slug " + slug + ". Does it exist?");
        return null;
    }

    public static void UpdatePlayerStatsFromID(int id, float health, float hunger, float thirst)
    {
        Stats s = GetPlayerObjectFromID(id).GetComponent<Stats>();
        s.UpdateStats(health, hunger, thirst);
    }

    public static void SetMenuCameraActive(bool value)
    {
        instance.menuCamera.enabled = value;
    }

    public static void DestroyAllPlayers()
    {
        for (int i = 0; i < instance.playerList.Count; i++)
        {
            Destroy(instance.playerList[i]);
        }
    }
}
