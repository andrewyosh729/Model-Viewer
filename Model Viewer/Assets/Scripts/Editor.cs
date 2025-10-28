using UnityEngine;

public abstract class Editor : MonoBehaviour
{
    public abstract string EditorTag { get; }

    protected virtual void Start()
    {
        Populate();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        Populate();
        gameObject.SetActive(true);
    }

    private void Populate()
    {
        OnPopulate();
    }

    protected abstract void OnPopulate();
}