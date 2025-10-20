using UnityEngine;

public abstract class GizmoHandle : MonoBehaviour
{
    [SerializeField] public Axis Axis;
    protected Gizmo Gizmo { get; private set; }
    public bool IsInteracting { get; private set; }

    private void Start()
    {
        Gizmo = transform.GetComponentInParent<Gizmo>();
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

    protected virtual Plane GetHandlePlane(Vector3 axis)
    {
        Vector3 camForward = Camera.main.transform.forward;

        // Generate a stable normal for the plane using cross product logic
        Vector3 planeNormal = Vector3.Cross(Vector3.Cross(axis, camForward), axis).normalized;

        return new Plane(planeNormal, transform.position);
    }
}