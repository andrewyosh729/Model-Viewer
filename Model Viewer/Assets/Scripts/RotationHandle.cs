using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotationHandle : GizmoHandle
{
    private Vector3 PreviousHitVector { get; set; }

    private float AccumulatedRotationDelta { get; set; }

    protected override void OnBeginInteraction()
    {
        base.OnBeginInteraction();
        Vector3 rotationAxis = Axis switch
        {
            Axis.X => Gizmo.Target.transform.right,
            Axis.Y => Gizmo.Target.transform.up,
            Axis.Z => Gizmo.Target.transform.forward,
            _ => throw new ArgumentOutOfRangeException()
        };
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane rotationPlane = GetHandlePlane(rotationAxis);
        if (rotationPlane.Raycast(ray, out float distance))
        {
            PreviousHitVector = ray.GetPoint(distance) - transform.position;
            PreviousHitVector = Vector3.ProjectOnPlane(PreviousHitVector, rotationPlane.normal);
        }

        AccumulatedRotationDelta = 0;
    }

    protected override Plane GetHandlePlane(Vector3 axis)
    {
        return new Plane(axis, transform.position);
    }

    public override void UpdateHandle()
    {
        Vector3 rotationAxis = Axis switch
        {
            Axis.X => Gizmo.Target.transform.right,
            Axis.Y => Gizmo.Target.transform.up,
            Axis.Z => Gizmo.Target.transform.forward,
            _ => throw new ArgumentOutOfRangeException()
        };
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        Plane rotationPlane = GetHandlePlane(rotationAxis);
        if (rotationPlane.Raycast(ray, out float distance))
        {
            Vector3 hit = ray.GetPoint(distance);
            Vector3 currentVector = hit - transform.position;
            currentVector = Vector3.ProjectOnPlane(currentVector, rotationPlane.normal);
            float deltaAngle =
                Vector3.SignedAngle(PreviousHitVector.normalized, currentVector.normalized, rotationAxis);
            AccumulatedRotationDelta += deltaAngle;

            if (Gizmo.SnapInterval <= 0)
            {
                Gizmo.Target.rotation = Quaternion.AngleAxis(deltaAngle, rotationAxis) * Gizmo.Target.rotation;
            }
            else if (Mathf.Abs(AccumulatedRotationDelta) >= Gizmo.SnapInterval)
            {
                Gizmo.Target.rotation =
                    Quaternion.AngleAxis(Mathf.Sign(AccumulatedRotationDelta) * Gizmo.SnapInterval, rotationAxis) *
                    Gizmo.Target.rotation;
                AccumulatedRotationDelta = 0;
            }

            PreviousHitVector = currentVector;
        }
    }

    protected override void OnEndInteraction()
    {
        base.OnEndInteraction();
        Gizmo.transform.rotation = Gizmo.Target.rotation;
    }
}