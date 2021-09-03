using System;
using UnityEngine;

public class LocalMainMenuManager : MonoBehaviour
{
    [SerializeField] private MyNetworkManager networkManager;

    [Header("UI")] 
    [SerializeField] private GameObject playerNameInputPanel;
    [SerializeField] private GameObject landingPagePanel;
    [SerializeField] private GameObject ipAddressPanel;

    private void OnEnable()
    {
        MyNetworkManager.OnClientDisconnected += OnClientDisconnect;
    }

    private void OnDisable()
    {
        MyNetworkManager.OnClientDisconnected -= OnClientDisconnect;
    }

    private void Start()
    {
        playerNameInputPanel.SetActive(true);
        landingPagePanel.SetActive(false);
        ipAddressPanel.SetActive(false);
    } //sets the ui to be correct no matter what is enabled in the editor for ease during the development

    private void OnClientDisconnect()
    {
        landingPagePanel.SetActive(true);
    } //activates the main set of buttons if you're disconnected from the server

    public void HostLobby()
    {
        networkManager.StartHost();
        landingPagePanel.SetActive(false);
    } // starts a hosted game and disables the main set of buttons

    public void ExitGame()
    {
        Application.Quit();
    }
}
