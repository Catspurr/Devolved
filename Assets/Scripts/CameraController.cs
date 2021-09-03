using UnityEngine;
using Mirror;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private Transform cam, target, player;
    private float _mouseX, _mouseY, _minCamAngle = -20f, _maxCamAngle = 60f;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void LateUpdate()
    {
        CameraControl();
    }

    private void CameraControl()
    {
        if(!isLocalPlayer) return;
        _mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
        _mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        _mouseY = Mathf.Clamp(_mouseY, _minCamAngle, _maxCamAngle);
        
        cam.LookAt(target);
        
        target.rotation = Quaternion.Euler(_mouseY, _mouseX,0);
        player.rotation = Quaternion.Euler(0, _mouseX,0);
    }
}
