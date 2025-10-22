using UnityEngine;
using VContainer;

public class LightFactory : MonoBehaviour
{
    [Inject] private InputService InputService { get; set; }
    [SerializeField] private GameObject LightPrefab;


    private void Update()
    {
        if (InputService.CameraControls.AddLight.LKey.triggered)
        {
            Camera camera = Camera.main;
            Instantiate(LightPrefab, camera.transform.position + camera.transform.forward * 5,
                Quaternion.identity);
        }
    }
}
