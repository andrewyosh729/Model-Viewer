using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;


public class GizmoService : MonoBehaviour
{
    [Inject] private InputService InputService { get; set; }
    [SerializeField] private GizmoType ActiveGizmoType = GizmoType.Scale;
    [SerializeField] private List<Gizmo> Gizmos;
    private Transform m_selectedObject;
    private Gizmo ActiveGizmo { get; set; }
    private Ray MouseRay => Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());


    private Transform SelectedObject
    {
        set
        {
            m_selectedObject = value;
            SetAllGizmosInactive();

            // unselected
            if (m_selectedObject == null)
            {
                ActiveGizmo = null;
                return;
            }

            ActivateGizmo(ActiveGizmoType, m_selectedObject);
        }
    }

    private void SetAllGizmosInactive()
    {
        foreach (Gizmo gizmo in Gizmos)
        {
            gizmo.gameObject.SetActive(false);
        }
    }

    private void ActivateGizmo(GizmoType gizmoType, Transform resizeTarget)
    {
        Gizmo gizmo = Gizmos.FirstOrDefault(g => g.Type == gizmoType);
        if (gizmo != null)
        {
            gizmo.gameObject.SetActive(true);
            gizmo.transform.position = m_selectedObject.position;
            gizmo.transform.localRotation = Quaternion.identity;
            gizmo.Target = resizeTarget;
            ActiveGizmo = gizmo;
        }
    }


    private void Start()
    {
        InputService.CameraControls.Select.MouseClick.started += MouseDown;
        InputService.CameraControls.Select.MouseClick.canceled += MouseUp;
    }

    private void Update()
    {
        if (ActiveGizmo)
        {
            transform.localScale = CalculateNewScale();
        }
    }

    private Vector3 CalculateNewScale()
    {
        Camera camera = Camera.main;
        float distance = Vector3.Distance(camera.transform.position, ActiveGizmo.Target.position);
        float fov = camera.fieldOfView * Mathf.Deg2Rad;
        float screenFraction = 0.1f;

        float scale = distance * Mathf.Tan(fov * 0.5f) * screenFraction;
        return Vector3.one * scale;
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
        try
        {
            if (ActiveGizmo != null && ActiveGizmo.Handles.Any(h => h.IsInteracting))
            {
                return;
            }


            SelectedObject = Physics.Raycast(MouseRay, out RaycastHit hit, float.MaxValue,
                1 << LayerMask.NameToLayer("Default"))
                ? hit.transform
                : null;
        }
        finally
        {
            if (ActiveGizmo)
            {
                foreach (GizmoHandle handle in ActiveGizmo.Handles)
                {
                    handle.EndInteraction();
                }
            }
        }
    }
}