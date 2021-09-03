using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private void Awake() => PlayerSpawnSystem.AddSpawnPoint(transform);
    private void OnDestroy() => PlayerSpawnSystem.RemoveSpawnPoint(transform);
} //Sits on the spawn point game object, adds the point to the PlayerSpawnSystem list of spawn points
