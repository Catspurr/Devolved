using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private static List<Transform> spawnPoints = new List<Transform>();

    private int nextIndex = 0;

    public static void AddSpawnPoint(Transform transform) //adds spawn points to the list, called from the actual spawn point
    {
        spawnPoints.Add(transform);
        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList(); //makes sure the list is ordered
    }

    public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform); //removed spawn points form the list, called from the spawn point

    public override void OnStartServer() => MyNetworkManager.OnServerReadied += SpawnPlayer; // once the server is ready, spawn players

    [ServerCallback]
    private void OnDestroy() => MyNetworkManager.OnServerReadied -= SpawnPlayer; //unsubscribe from the event when game object is destroyed

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        var spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex); //sets a transform point from the list of spawn points to check if its valid

        if (spawnPoint == null)
        {
            Debug.LogError($"Missing spawn point for player {nextIndex}!");
            return;
        }

        var playerInstance = Instantiate(
            playerPrefab, 
            spawnPoints[nextIndex].position, 
            spawnPoints[nextIndex].rotation); //Instantiates a game object at the spawn point
        NetworkServer.Spawn(playerInstance, conn); //Sets the game object instance to be connected to the specific client

        nextIndex++; //once one player is spawned we increase the index so the next player spawn on the next point
    }
}
