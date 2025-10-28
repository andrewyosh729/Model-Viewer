public class RotationGizmo : Gizmo
{
    public override GizmoType Type => GizmoType.Rotation;
    public override float SnapInterval { get; set; } = 15f;
}