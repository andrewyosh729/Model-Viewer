using System;
using JetBrains.Annotations;
using Services;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class Menu : MonoBehaviour
{
    [Inject] private IModelImportService ModelImportService { get; set; }

    [Inject] private GizmoService GizmoService { get; set; }
    [Inject] private LightFactory LightFactory { get; set; }
    [Inject] private InputService InputService { get; set; }

    [SerializeField] private RawImage PositionGizmoImage;
    [SerializeField] private RawImage RotationGizmoImage;
    [SerializeField] private RawImage ScaleGizmoImage;

    private void Awake()
    {
        GizmoSelected(GizmoService.ActiveGizmoType);
    }

    [UsedImplicitly]
    public void ImportModel()
    {
        if (FileDialogUtils.TryOpenFile(out string path))
        {
            ModelImportService.ImportModel(path);
        }
    }

    private void GizmoSelected(GizmoType type)
    {
        switch (type)
        {
            case GizmoType.None:
                PositionGizmoImage.color = Color.grey;
                RotationGizmoImage.color = Color.grey;
                ScaleGizmoImage.color = Color.grey;
                break;
            case GizmoType.Position:
                PositionGizmoImage.color = Color.white;
                RotationGizmoImage.color = Color.grey;
                ScaleGizmoImage.color = Color.grey;
                break;
            case GizmoType.Rotation:
                PositionGizmoImage.color = Color.grey;
                RotationGizmoImage.color = Color.white;
                ScaleGizmoImage.color = Color.grey;
                break;
            case GizmoType.Scale:
                PositionGizmoImage.color = Color.grey;
                RotationGizmoImage.color = Color.grey;
                ScaleGizmoImage.color = Color.white;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        GizmoService.ActivateGizmo(type, InputService.SelectedObject);
    }

    [UsedImplicitly]
    public void CreateLight()
    {
        LightFactory.CreateLight();
    }

    public void GizmoSelected(GameObject gizmoPreview)
    {
        GizmoType gizmoType = gizmoPreview.GetComponent<GizmoSelection>().Type;
        GizmoSelected(gizmoType);
    }
}