using UnityEngine;

public abstract class GizmoHandle : MonoBehaviour
{
    [SerializeField] public Axis Axis;
    protected Gizmo Gizmo { get; private set; }
    public bool IsInteracting { get; private set; }

    private void Start()
    {
        Gizmo = transform.GetComponentInParent<ScaleGizmo>();
    }

    public void BeginInteraction()
    {
        IsInteracting = true;
        OnBeginInteraction();
    }

    public void EndInteraction()
    {
        if (IsInteracting)
        {
            IsInteracting = false;
            OnEndInteraction();
        }
    }

    protected virtual void OnBeginInteraction()
    {
    }

    protected virtual void OnEndInteraction()
    {
    }

    private void Update()
    {
        if (IsInteracting)
        {
            UpdateHandle();
        }
    }

    public abstract void UpdateHandle();
}