using UnityEngine;
using VContainer;

public class ModelEditor : Editor
{
    [SerializeField] private TMPro.TMP_Text NameText;

    [Inject] private InputService InputService;
    public override string EditorTag => "Model";


    // [UsedImplicitly]
    // public void UpdateTexture(string textureName)
    // {
    //     if (!InputService.SelectedObject)
    //     {
    //         return;
    //     }
    //
    //     if (!FileDialogUtils.TryOpenFile(out string path))
    //     {
    //         return;
    //     }
    //
    //     if (!File.Exists(path))
    //     {
    //         return;
    //     }
    //
    //
    //     Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false, true);
    //     texture.LoadImage(File.ReadAllBytes(path), true);
    //
    //
    //     foreach (MeshRenderer renderer in InputService.SelectedObject.GetComponentsInChildren<MeshRenderer>())
    //     {
    //         renderer.material.SetTexture(textureName, texture);
    //     }
    // }


    protected override void OnPopulate()
    {
        NameText.text = InputService.SelectedObject.name;
    }
}