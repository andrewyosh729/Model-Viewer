using System.Collections.Generic;
using UnityEngine;

public abstract class Gizmo : MonoBehaviour
{
    [SerializeField] protected List<GizmoHandle> GizmoHandles;

    public IReadOnlyList<GizmoHandle> Handles => GizmoHandles;

    public Transform Target { get; set; }

    public abstract GizmoType Type { get; }

    public virtual float SnapInterval => 0;


    private void Update()
    {
        if (Target)
        {
            UpdateTransform();
        }
    }

    private void UpdateTransform()
    {
        transform.position = Target.position;
        transform.rotation = Target.rotation;
    }
}