using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VContainer;

public class InputService : MonoBehaviour
{
    public CameraControls CameraControls { get; private set; }
    [Inject] private GizmoService GizmoService { get; set; }

    private Ray MouseRay => Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

    private Transform m_selectedObject;

    public Transform SelectedObject
    {
        get => m_selectedObject;
        private set
        {
            m_selectedObject = value;

            // unselected
            if (m_selectedObject == null)
            {
                GizmoService.ActivateGizmo(null, m_selectedObject);
                return;
            }

            GizmoService.ActivateGizmo(GizmoService.ActiveGizmoType, m_selectedObject);
        }
    }


    private void Awake()
    {
        CameraControls = new CameraControls();
        CameraControls.Enable();
    }

    private void Start()
    {
        CameraControls.Select.MouseClick.started += MouseDown;
        CameraControls.Select.MouseClick.canceled += MouseUp;
    }

    private void Update()
    {
        if (CameraControls.Delete.DeleteKey.triggered && SelectedObject)
        {
            Destroy(SelectedObject.gameObject);
            SelectedObject = null;
        }
    }

    private void MouseDown(InputAction.CallbackContext obj)
    {
        if (Physics.Raycast(MouseRay, out RaycastHit gizmoHit, float.MaxValue, 1 << LayerMask.NameToLayer("Gizmo")))
        {
            if (gizmoHit.transform.TryGetComponent(out GizmoHandle gizmoHandle))
            {
                gizmoHandle.BeginInteraction();
            }
        }
    }

    private void MouseUp(InputAction.CallbackContext obj)
    {
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject(Mouse.current.deviceId))
        {
            return;
        }

        try

        {
            if (GizmoService.InteractingWithGizmo)
            {
                return;
            }

            if (Physics.Raycast(MouseRay, out RaycastHit hit, float.MaxValue,
                    1 << LayerMask.NameToLayer("Default")))
            {
                Transform current = hit.transform;
                while (true)
                {
                    if (!current.parent)
                    {
                        break;
                    }

                    current = current.parent;
                }

                SelectedObject = current;
            }
            else
            {
                SelectedObject = null;
            }
        }
        finally
        {
            GizmoService.EndGizmoInteraction();
        }
    }

    private void OnDestroy()
    {
        CameraControls.Disable();
    }
}