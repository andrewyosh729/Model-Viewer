using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class ResizeHandle : GizmoHandle
{
    [Inject] private InputService InputService { get; set; }


    private Vector3 ResizeDirection => transform.up.normalized;
    private Vector3 InitialTargetScale { get; set; }
    private Vector3 InitialHandleLossyScale { get; set; }


    protected override void OnBeginInteraction()
    {
        base.OnBeginInteraction();
        InitialTargetScale = Gizmo.Target.localScale;
        InitialHandleLossyScale = transform.lossyScale;
    }

    public override void UpdateHandle()
    {
        Vector3 cameraOffset = Camera.main.transform.position - Gizmo.Target.position;
        Vector3 handleAxis = transform.up;
        Plane axisPlane = new Plane(handleAxis, Gizmo.Target.position);
        Vector3 projectedOffset = axisPlane.ClosestPointOnPlane(cameraOffset);
        Plane resizePlane = new Plane(projectedOffset, Gizmo.Target.position);
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Mouse.current.position.value.x,
            Mouse.current.position.value.y, 0));
        if (resizePlane.Raycast(ray, out float distance))
        {
            Vector3 hit = ray.GetPoint(distance);
            Vector3 dragVector = hit - transform.position;

            float newScale = Vector3.Dot(dragVector, ResizeDirection);
            newScale /= 2; // Unity scales symmetrically around pivot — divide by 2 to match one-sided drag
            Vector3 resizeTargetScale = Vector3.one;
            Vector3 resizeHandleScale = transform.localScale;
            resizeHandleScale.y = newScale * (resizeHandleScale.y / transform.lossyScale.y);
            switch (Axis)
            {
                case Axis.X:
                    resizeTargetScale.x = newScale / InitialHandleLossyScale.x;
                    break;
                case Axis.Y:
                    resizeTargetScale.y = newScale / InitialHandleLossyScale.y;
                    break;
                case Axis.Z:
                    resizeTargetScale.z = newScale / InitialHandleLossyScale.z;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            transform.localScale = resizeHandleScale;
            resizeTargetScale.Scale(InitialTargetScale);
            Gizmo.Target.localScale = resizeTargetScale;
        }
    }

    protected override void OnEndInteraction()
    {
        base.OnEndInteraction();
        transform.localScale = Vector3.one;
    }
}