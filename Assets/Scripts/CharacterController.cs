using System;
using UnityEngine;
using Mirror;

public class CharacterController : NetworkBehaviour
{
    [SerializeField] private float playerSpeed = 10f;
    [SerializeField] private GameObject playerCamera;

    public override void OnStartLocalPlayer()
    {
        playerCamera.SetActive(true);
    }

    private void Awake()
    {
        try
        {
            Camera.main.gameObject.SetActive(false);
        }
        catch
        {
            Debug.Log("No main camera found in scene!");
        }
        AddPlayer();
    }

    private void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        if (!isLocalPlayer) return;
        var hor = Input.GetAxis("Horizontal");
        var ver = Input.GetAxis("Vertical");
        var movement = new Vector3(hor, 0f, ver) * (playerSpeed * Time.deltaTime); //Might need changing?
        transform.Translate(movement, Space.Self);
    }

    [Command]
    private void AddPlayer()
    {
        EventHandler.Current.ClientJoin();
    }
}
