using UnityEngine;

public class InputService : MonoBehaviour
{
    
    public CameraControls CameraControls { get; private set; }

    private void Awake()
    {
        CameraControls = new CameraControls();
        CameraControls.Enable();
    }

    private void OnDestroy()
    {
        CameraControls.Disable();
    }
}
