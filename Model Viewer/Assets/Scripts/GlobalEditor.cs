using System.Linq;
using TMPro;
using UnityEngine;
using VContainer;

public class GlobalEditor : Editor
{
    [SerializeField] private TMP_InputField PositionSnapInputField;
    [SerializeField] private TMP_InputField RotationSnapInputField;
    [Inject] private IGizmoService GizmoService { get; set; }
    public override string EditorTag => null;

    private Gizmo m_PositionGizmo;

    private Gizmo PositionGizmo =>
        m_PositionGizmo ??= GizmoService.Gizmos.FirstOrDefault(g => g.Type == GizmoType.Position);

    private Gizmo m_RotationGizmo;

    private Gizmo RotationGizmo =>
        m_RotationGizmo ??= GizmoService.Gizmos.FirstOrDefault(g => g.Type == GizmoType.Rotation);

    protected override void Start()
    {
        base.Start();
        PositionSnapInputField.onSubmit.AddListener(PositionSnapInputFieldSubmitOrLoseFocus);
        PositionSnapInputField.onDeselect.AddListener(PositionSnapInputFieldSubmitOrLoseFocus);

        RotationSnapInputField.onSubmit.AddListener(RotationSnapInputFieldSubmitOrLoseFocus);
        RotationSnapInputField.onDeselect.AddListener(RotationSnapInputFieldSubmitOrLoseFocus);
    }

    private void PositionSnapInputFieldSubmitOrLoseFocus(string arg0)
    {
        if (float.TryParse(arg0, out float snapValue))
        {
            PositionGizmo.SnapInterval = snapValue;
            return;
        }

        PositionSnapInputField.text = PositionGizmo.SnapInterval.ToString();
    }

    private void RotationSnapInputFieldSubmitOrLoseFocus(string arg0)
    {
        if (float.TryParse(arg0, out float snapValue))
        {
            RotationGizmo.SnapInterval = snapValue;
            return;
        }

        RotationSnapInputField.text = RotationGizmo.SnapInterval.ToString();
    }

    protected override void OnPopulate()
    {
        PositionSnapInputField.text = PositionGizmo.SnapInterval.ToString();
        RotationSnapInputField.text = RotationGizmo.SnapInterval.ToString();
    }
}