using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [Scene] [SerializeField] private string menuScene = string.Empty;

    [Header("Room")] 
    [SerializeField] private NetworkRoomPlayerDevolved roomPlayerPrefab;

    [Header("Game")] 
    [SerializeField] private NetworkGamePlayerDevolved gamePlayerPrefab;
    [SerializeField] private GameObject playerSpawnSystem;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;

    public List<NetworkRoomPlayerDevolved> RoomPlayers { get; } = 
        new List<NetworkRoomPlayerDevolved>();
    public List<NetworkGamePlayerDevolved> GamePlayers { get; } = 
        new List<NetworkGamePlayerDevolved>();

    //==============CHANGE==================
    public override void OnStartServer() => spawnPrefabs = 
        Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");
        foreach (var prefab in spawnablePrefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
    }
    //=======================================
    //Loads all prefabs in SpawnablePrefabs folder inside the Resources folder, 

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        OnClientConnected?.Invoke();
    } //Does the base functions plus invokes an event whenever a client connects to the server client side

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        OnClientDisconnected?.Invoke();
    } //Does the base functions plus invokes an event whenever a client disconnects to the server client side

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        } //Disconnects the new connection if the current amount of connections is the same or more than the max amount of connections

        if (SceneManager.GetActiveScene().path != menuScene)
        {
            conn.Disconnect();
            return;
        } //Disconnects the connection if they arent on the correct scene
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            var isLeader = RoomPlayers.Count == 0;
            
            var roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader; //sets the roomPlayer (for lobby) to be leader or not depending on their place in the list. 0 = leader
            
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject); //adds the instanced game object to the current connection
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayerDevolved>();

            RoomPlayers.Remove(player); //removes the player from the list

            NotifyPlayersOfReadyState(); //Updates all client but only runs on the leader client to enable or disable the start button
        }
        
        base.OnServerDisconnect(conn); //Runs the base function of disconnecting
    }

    public override void OnStopServer()
    {
        RoomPlayers.Clear(); //clears the list to prevent overlapping
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) return false; //if there is not enough players we are not ready to start
        foreach (var player in RoomPlayers)
        {
            if (!player.IsReady) return false;
        } //if any player is not ready we are not ready

        return true; //if we have enough players and all players are ready then we are ready
    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            if (!IsReadyToStart()) return;
            
            ServerChangeScene("GameScene"); //Calls the server to change the scene into the game scene if we are on the right scene and all players are ready
        }
    }

    public override void ServerChangeScene(string sceneName)
    {
        if (SceneManager.GetActiveScene().path == menuScene && sceneName.StartsWith("Game")) //if we move from lobby to a game scene
        {
            for (var i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gamePlayerInstance = Instantiate(gamePlayerPrefab);
                gamePlayerInstance.SetDisplayName(RoomPlayers[i].DisplayName); //Instantiates the game player
                
                NetworkServer.Destroy(conn.identity.gameObject); //removes the room player

                NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject); //sets the instantiated game player to be the new game object for the client
            }
        }
        
        base.ServerChangeScene(sceneName); //runs the base function after creating the new game player
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.StartsWith("Game")) //if the scene name starts with Game we know its a gameplay scene
        {
            var playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance); //Instantiates and spawns the spawn system to spawn players
        }
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
        OnServerReadied?.Invoke(conn);
    } //runs the base function and invokes an event
}
