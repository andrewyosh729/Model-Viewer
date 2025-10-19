using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TranslationHandle : GizmoHandle
{
    private Vector3 TranslationDirection => transform.up.normalized;

    private Vector3? PreviousHitPosition { get; set; }

    protected override void OnBeginInteraction()
    {
        base.OnBeginInteraction();
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane translationPlane = GetHandlePlane(TranslationDirection);
        if (translationPlane.Raycast(ray, out float distance))
        {
            PreviousHitPosition = ray.GetPoint(distance);
        }
    }

    public override void UpdateHandle()
    {
        if (PreviousHitPosition == null)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane translationPlane = GetHandlePlane(TranslationDirection);
        if (translationPlane.Raycast(ray, out float distance))
        {
            Vector3 hit = ray.GetPoint(distance);
            Vector3 dragVector = hit - PreviousHitPosition.Value;

            float translationDelta = Vector3.Dot(dragVector, TranslationDirection);
            Vector3 translationTargetPosition = Gizmo.Target.position;
            switch (Axis)
            {
                case Axis.X:
                    translationTargetPosition.x += translationDelta;
                    break;
                case Axis.Y:
                    translationTargetPosition.y += translationDelta;
                    break;
                case Axis.Z:
                    translationTargetPosition.z -= translationDelta;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Gizmo.Target.position = translationTargetPosition;
            PreviousHitPosition = hit;
        }
    }

    protected override void OnEndInteraction()
    {
        base.OnEndInteraction();
        PreviousHitPosition = null;
    }
}