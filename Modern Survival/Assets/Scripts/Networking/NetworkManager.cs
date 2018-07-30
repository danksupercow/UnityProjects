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
    public int connectionID;
    public TcpClient Socket { get { return clientTCP.playerSocket; } }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        UnityThread.initUnityThread();
        instance = this;
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
        if(index != connectionID)
        {
            PlayerController ply = temp.GetComponent<PlayerController>();

            Destroy(temp.GetComponent<ViewController>());
            Destroy(ply.transform.Find("PlayerCamera").gameObject);
            Destroy(ply);

        }
        playerList.Add(index, temp);
    }

    public GameObject GetPlayerObjectFromID(int id)
    {
        return playerList[id];
    }

}
