using UnityEngine;
using UnityEngine.InputSystem;

public class UniformResizeHandle : GizmoHandle
{
    private Vector3 InitialTargetScale { get; set; }
    private Vector3 InitialHandleLossyScale { get; set; }
    private Vector3 InitialHandleLocalScale { get; set; }
    private Vector3? InitialHitPoint { get; set; }

    private void Awake()
    {
        InitialHandleLocalScale = transform.localScale;
    }

    protected override void OnBeginInteraction()
    {
        base.OnBeginInteraction();
        InitialTargetScale = Gizmo.Target.localScale;
        InitialHandleLossyScale = transform.lossyScale;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane resizePlane = GetHandlePlane(transform.up);
        if (resizePlane.Raycast(ray, out float distance))
        {
            InitialHitPoint = ray.GetPoint(distance);
        }
    }

    public override void UpdateHandle()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane resizePlane = GetHandlePlane(transform.up);
        if (resizePlane.Raycast(ray, out float distance))
        {
            Vector3 hit = ray.GetPoint(distance);
            Vector3 dragVector = hit - (InitialHitPoint ?? transform.position);

            float rightDrag = Vector3.Dot(dragVector, Camera.main.transform.right);
            float upDrag = Vector3.Dot(dragVector, Camera.main.transform.up);
            float dragDistance = rightDrag + upDrag;
            float newScaleFactor = 1 + (dragDistance / (2 * InitialHandleLossyScale.y));
            newScaleFactor = Mathf.Clamp(newScaleFactor, float.Epsilon, float.MaxValue);

            Gizmo.Target.localScale = InitialTargetScale * newScaleFactor;
        }
    }

    protected override void OnEndInteraction()
    {
        base.OnEndInteraction();
        transform.localScale = InitialHandleLocalScale;
    }
}