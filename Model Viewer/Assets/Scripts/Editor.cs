using UnityEngine;
using VContainer;

public abstract class Editor : MonoBehaviour
{
    [Inject] protected InputService InputService { get; set; }

    protected abstract string EditorTag { get; }

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
        if (!InputService.SelectedObject || !InputService.SelectedObject.CompareTag(EditorTag))
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        OnPopulate();
    }

    protected abstract void OnPopulate();
}