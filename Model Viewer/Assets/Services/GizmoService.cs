using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VContainer;


public class GizmoService : MonoBehaviour
{
    [NonSerialized] public GizmoType ActiveGizmoType = GizmoType.Scale;
    public bool InteractingWithGizmo => ActiveGizmo != null && ActiveGizmo.Handles.Any(h => h.IsInteracting);

    [SerializeField] private List<Gizmo> Gizmos;
    private Gizmo ActiveGizmo { get; set; }


    public void EndGizmoInteraction()
    {
        if (ActiveGizmo)
        {
            foreach (GizmoHandle handle in ActiveGizmo.Handles)
            {
                handle.EndInteraction();
            }
        }
    }

    private void SetAllGizmosInactive()
    {
        foreach (Gizmo gizmo in Gizmos)
        {
            gizmo.gameObject.SetActive(false);
        }
    }

    public void ActivateGizmo(GizmoType? gizmoType, Transform target)
    {
        SetAllGizmosInactive();

        if (gizmoType == null)
        {
            return;
        }

        ActiveGizmoType = gizmoType.Value;

        if (!target)
        {
            return;
        }

        Gizmo gizmo = Gizmos.FirstOrDefault(g => g.Type == gizmoType);
        if (gizmo != null)
        {
            gizmo.gameObject.SetActive(true);
            gizmo.transform.position = target.transform.position;
            gizmo.transform.localRotation = Quaternion.identity;
            gizmo.Target = target.transform;
            ActiveGizmo = gizmo;
        }
    }


    private void Update()
    {
        if (ActiveGizmo?.Target)
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
}