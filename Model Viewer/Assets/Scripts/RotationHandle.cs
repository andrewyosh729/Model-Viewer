using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotationHandle : GizmoHandle
{
    private Vector3 PreviousHitVector { get; set; }


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
            Debug.Log(deltaAngle);
            Gizmo.Target.rotation = Quaternion.AngleAxis(deltaAngle, rotationAxis) * Gizmo.Target.rotation;

            PreviousHitVector = currentVector;
            Debug.DrawRay(transform.position, rotationAxis * 2f, Color.blue); // Rotation axis
            Debug.DrawRay(transform.position, PreviousHitVector * 2f, Color.red); // Previous vector
            Debug.DrawRay(transform.position, currentVector * 2f, Color.green); // Current vector        }
        }
    }

    protected override void OnEndInteraction()
    {
        base.OnEndInteraction();
        Gizmo.transform.rotation = Gizmo.Target.rotation;
    }
}