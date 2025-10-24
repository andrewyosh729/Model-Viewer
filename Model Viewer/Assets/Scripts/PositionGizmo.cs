public class PositionGizmo : Gizmo
{
    public override GizmoType Type => GizmoType.Position;
    public override float SnapInterval => 1f;
}