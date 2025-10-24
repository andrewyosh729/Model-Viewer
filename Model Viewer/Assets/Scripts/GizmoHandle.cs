using UnityEngine;

public abstract class GizmoHandle : MonoBehaviour
{
    [SerializeField] public Axis Axis;
    protected Gizmo Gizmo { get; private set; }
    public bool IsInteracting { get; private set; }
    public bool IsHovering { get; private set; }

    private Color? m_BaseColor;
    private Color BaseColor => m_BaseColor ??= GetComponent<Renderer>().material.GetColor("_Color");

    private Renderer m_Renderer;
    private Renderer Renderer => m_Renderer ??= GetComponent<Renderer>();

    private void Start()
    {
        Gizmo = transform.GetComponentInParent<Gizmo>();
        SetAlpha(0.5f);
    }

    public void BeginHover()
    {
        IsHovering = true;
        OnBeginHover();
    }

    public void EndHover()
    {
        IsHovering = false;
        OnEndHover();
    }

    protected virtual void OnBeginHover()
    {
        SetAlpha(1f);
    }

    protected virtual void OnEndHover()
    {
        SetAlpha(0.5f);
    }

    private void SetAlpha(float alpha)
    {
        Color color = BaseColor;
        color.a = alpha;
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_Color", color);
        Renderer.SetPropertyBlock(block);
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