using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private MyNetworkManager networkManager;

    [Header("UI")] 
    [SerializeField] private GameObject landingPagePanel;
    [SerializeField] private TMP_InputField ipAddressInputField;
    [SerializeField] private Button joinButton;

    private void OnEnable()
    {
        MyNetworkManager.OnClientConnected += HandleClientConnected;
        MyNetworkManager.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        MyNetworkManager.OnClientConnected -= HandleClientConnected;
        MyNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby()
    {
        var ipAddress = ipAddressInputField.text;
        networkManager.networkAddress = ipAddressInputField.text; //sets the ip address to whats in the input field
        networkManager.StartClient(); //starts trying to connect to the ip

        joinButton.interactable = false; //disable the join button while trying to connect to the ip
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true; //resets the join button
        
        gameObject.SetActive(false); //disables the ip input window
        landingPagePanel.SetActive(false); //disables the main menu buttons
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true; //enables the join button if we fail to connect to the server
    }
}
