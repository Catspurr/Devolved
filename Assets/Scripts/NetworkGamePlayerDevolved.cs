using Mirror;

public class NetworkGamePlayerDevolved : NetworkBehaviour
{
    [SyncVar]
    private string displayName = "Loading..."; //synced on the server

    private MyNetworkManager room;
    private MyNetworkManager Room
    {
        get
        {
            if (room != null) return room;
            return room = NetworkManager.singleton as MyNetworkManager;
        }
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);
        
        Room.GamePlayers.Add(this); //adds this to the GamePlayer list on the network manager
    }

    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this); //removes this from the list in the network manager
    }

    [Server]
    public void SetDisplayName(string dName) //called on server to set the display name
    {
        displayName = dName;
    }
}
