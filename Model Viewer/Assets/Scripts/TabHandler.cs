using UnityEngine;

public class TabHandler : MonoBehaviour
{
    [SerializeField] private GameObject PanelToControl;

    public void TogglePanel()
    {
        PanelToControl.SetActive(!PanelToControl.activeSelf);
    }
}