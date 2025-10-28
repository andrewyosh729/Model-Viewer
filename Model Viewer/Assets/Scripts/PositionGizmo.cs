public class PositionGizmo : Gizmo
{
    public override GizmoType Type => GizmoType.Position;
    public override float SnapInterval { get; set; } = 0.1f;
}