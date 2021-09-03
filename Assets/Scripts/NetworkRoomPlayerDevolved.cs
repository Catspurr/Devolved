using Mirror;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomPlayerDevolved : NetworkBehaviour
{
    [Header("UI")] 
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[4];
    [SerializeField] private Button startGameButton;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";

    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    private bool _isLeader;
    public bool IsLeader
    {
        set
        {
            _isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }

    private MyNetworkManager room;
    private MyNetworkManager Room
    {
        get
        {
            if (room != null) return room;
            return room = NetworkManager.singleton as MyNetworkManager;
        }
    }

    public override void OnStartAuthority() //called for the object the client have authority over
    {
        CmdSetDisplayName(PlayerNameInput.DisplayName); //updates the name to the server
        lobbyUI.SetActive(true); //sets the ui active for the clients own object
    }

    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this); //adds this room player to the list of room players in network manager

        UpdateDisplay(); //initial update of the UI to show the players in the lobby
    }

    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);
        UpdateDisplay();
    } //removes the disconnected player from the network list and updates ui

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach (var player in Room.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        } //if we dont have authority check for the object that has authority and run on that client

        for (var i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting for player...";
            playerReadyTexts[i].text = string.Empty;
        } //resets all names and removes ready text

        for (var i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
            playerReadyTexts[i].text = Room.RoomPlayers[i].IsReady
                ? "<color=green>Ready</color>"
                : "<color=red>Not ready</color>";
        } //updates all names and ready tests for each player connected
    }

    public void HandleReadyToStart(bool b)
    {
        if (!_isLeader) return;

        startGameButton.interactable = b;
    } //sets the start game button to be interactable or not for the leader client

    [Command]
    private void CmdSetDisplayName(string dName)
    {
        DisplayName = dName;
    } //sets the name on server side

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;
        
        Room.NotifyPlayersOfReadyState();
    } //sets ready button as a toggle and updates the other clients of the state

    [Command]
    public void CmdStartGame()
    {
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient) return; //failsafe to check that its actually the leader
        
        Room.StartGame();
    } //starts the game
}
