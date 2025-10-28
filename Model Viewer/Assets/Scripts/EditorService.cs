using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using VContainer;

[UsedImplicitly]
public class EditorService : MonoBehaviour
{
    [Inject] protected InputService InputService { get; set; }

    [SerializeField] private Editor[] Editors;

    protected virtual void Start()
    {
        InputService.SelectedObjectUpdated += InputServiceOnSelectedObjectUpdated;
        Populate();
    }

    private void InputServiceOnSelectedObjectUpdated(Transform t)
    {
        Populate();
    }

    private void Populate()
    {
        foreach (Editor editor in Editors)
        {
            editor.Hide();
        }

        if (!InputService.SelectedObject)
        {
            Editors.FirstOrDefault(e => e.EditorTag == null)?.Show();
            return;
        }

        foreach (Editor editor in Editors)
        {
            if (editor.EditorTag == null)
            {
                continue;
            }

            if (InputService.SelectedObject.CompareTag(editor.EditorTag))
            {
                editor.Show();
            }
        }
    }
}