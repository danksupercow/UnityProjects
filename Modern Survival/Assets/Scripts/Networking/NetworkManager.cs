using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {

    public static NetworkManager instance;
    private ClientTCP clientTCP = new ClientTCP();
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject localPlayerPrefab;
    [SerializeField] private Camera menuCamera;
    [SerializeField] private Transform spawnPoint;
    public float positionSendThreshold = 0.5f;
    public float rotationSendThreshold = 0.08f;
    public Text connectionIDDisplay;

    /* ### Lists/Dictionaries... ### */
    //private Dictionary<int, Player> playerList = new Dictionary<int, Player>();

    public static Dictionary<int, SyncObject> SyncdObjects = new Dictionary<int, SyncObject>();
    //public static Dictionary<int, Animator> SyncdAnimations = new Dictionary<int, Animator>();

    private static List<ServerData> playerServers = new List<ServerData>();
    [SerializeField]
    private RegisteredPrefab[] registeredPrefabs;

    /* ### ID Shit... ### */
    public static int connectionID = -1;

    /* ### Properties... ### */
    public static TcpClient Socket { get { return instance.clientTCP.playerSocket; } }
    public static List<ServerData> PlayerServers { get { return playerServers; } }
    public static ClientTCP Client { get { return instance.clientTCP; } }
    public static Vector3 SpawnPosition { get { return instance.spawnPoint.position; } }
    public static float PosSendThreshold { get { return instance.positionSendThreshold; } }
    public static float RotSendThreshold { get { return instance.rotationSendThreshold; } }
    public static bool Connected { get { return instance.clientTCP.Connected; } }

    //Objects That Need To Be Destroyed From 'MainThread'
    private Queue<int> destroyQueue = new Queue<int>();

    private void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;

        UnityThread.initUnityThread();

        if(General.ReadPlayerData() != null)
        {
            playerServers = General.ReadPlayerData();
        }
        
    }

    private void OnApplicationQuit()
    {
        clientTCP.Disconnect();
    }
    
    private void Update()
    {
        connectionIDDisplay.text = connectionID.ToString();
        
        if(destroyQueue.Count > 0)
        {
            int destroyID = destroyQueue.Dequeue();
            Console.Log("Destroyed: " + SyncdObjects[destroyID].name);
            Destroy(SyncdObjects[destroyID].gameObject);
            SyncdObjects.Remove(destroyID);
        }

        if(!clientTCP.Connected && !menuCamera.enabled)
        {
            menuCamera.enabled = true;
        }
    }

    public GameObject InstantiatePlayer(int index)
    {
        GameObject temp = null;

        try
        {
            Console.LogError(index);
            if (index != connectionID)
            {
                temp = Instantiate(playerPrefab, new Vector3(0,2,0), Quaternion.identity);
                Console.Log("NonLocal");
            }
            else
            {
                temp = Instantiate(localPlayerPrefab, new Vector3(0, 2, 0), Quaternion.identity);
                menuCamera.enabled = false;
                Console.Log("Local");
            }

            SyncObject so = temp.GetComponent<SyncObject>();
            so.Init();

            SyncdObjects.Add(index, so);
        }
        catch (System.Exception e)
        {
            Console.LogError(e);
        }
        return temp;
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
    
    public static void SetMenuCameraActive(bool value)
    {
        instance.menuCamera.enabled = value;
    }
    
    public static void CleanupScene()
    {
        for (int i = 0; i < instance.destroyQueue.Count; i++)
        {
            instance.destroyQueue.Enqueue(i);
        }
    }

    public void Connect()
    {
        clientTCP.Connect();
    }

    public void Disconnect()
    {
        clientTCP.Disconnect();
    }
    
    public static GameObject FetchRegisteredPrefabFrom(string slug)
    {
        RegisteredPrefab[] prefs = instance.registeredPrefabs;

        for (int i = 0; i < prefs.Length; i++)
        {
            if (prefs[i].slug == slug)
            {
                return prefs[i].prefab;
            }
        }

        return null;
    }
    public static GameObject FetchSpawnedPrefab(int syncID)
    {
        return SyncdObjects[syncID].gameObject;
    }

    //SyncPosition Updates
    public static void UpdateSyncdObject(int syncID, Vector3 pos)
    {
        SyncObject so = SyncdObjects[syncID];
        if (so.syncData.positionSync == null)
        {
            Debug.LogError("SyncObject: " + syncID + " Does Not Conatin A SyncPosition Component but You are Trying to Update One.");
            return;
        }
        so.syncData.positionSync.Receive(pos);
    }

    //SyncRotation Updates
    public static void UpdateSyncdObject(int syncID, Quaternion rot)
    {
        SyncObject so = SyncdObjects[syncID];
        if (so.syncData.rotationSync == null)
        {
            Debug.LogError("SyncObject: " + syncID + " Does Not Conatin A SyncRotation Component but You are Trying to Update One.");
            return;
        }
        so.syncData.rotationSync.Receive(rot);
    }

    //SyncAniamtor Updates
    public static void UpdateSyncdObject(int syncID, string id, bool value)
    {
        Animator animator = SyncdObjects[syncID].syncData.animatorSync.animator;
        animator.SetBool(id, value);
    }
    public static void UpdateSyncdObject(int syncID, string id, float value)
    {
        Animator animator = SyncdObjects[syncID].syncData.animatorSync.animator;
        animator.SetFloat(id, value);
    }
    public static void UpdateSyncdObject(int syncID, string id)
    {
        Animator animator = SyncdObjects[syncID].syncData.animatorSync.animator;
        animator.SetTrigger(id);
    }

    //Networked Object Handler
    public static SyncObject SpawnRegisteredPrefab(string slug, int assignedID)
    {
        GameObject temp = FetchRegisteredPrefabFrom(slug);

        if (temp == null)
        {
            Console.LogError("no prefab found");
            return null;
        }

        GameObject go = Instantiate(temp);
        SyncObject so = go.GetComponent<SyncObject>();
        if (so == null)
        {
            Debug.LogError("Failed to Create Registered Prefab: " + slug + " It Has No NetworkObject Component.");
            Destroy(go);
            return null;
        }

        so.syncData.netObj.data.ID = assignedID;
        SyncdObjects.Add(assignedID, so);
        return so;
    }
    public static void DestroySyncdPrefab(int syncID)
    {
        instance.destroyQueue.Enqueue(syncID);
    }

}
