using System;
using Services;
using UnityEngine;
using VContainer;

public class Menu : MonoBehaviour
{
    [Inject] private IModelImportService ModelImportService { get; set; }


    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 200, 90), "Menu");
        if (GUI.Button(new Rect(60, 40, 100, 20), "Import Model"))
        {
            if (FileDialogUtils.TryOpenFile(out string path))
            {
                ModelImportService.ImportModel(path);
            }
        }
    }
}