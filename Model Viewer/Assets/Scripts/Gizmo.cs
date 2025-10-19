using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Gizmo : MonoBehaviour
{
    [SerializeField] protected List<GizmoHandle> GizmoHandles;

    public IReadOnlyList<GizmoHandle> Handles => GizmoHandles;
    private Transform m_Target;

    public Transform Target
    {
        get => m_Target;
        set => m_Target = value;
    }

    public abstract GizmoType Type { get; }


    private void Update()
    {
        if (Target)
        {
            transform.position = Target.position;
            transform.rotation = Target.rotation;
        }
    }
}