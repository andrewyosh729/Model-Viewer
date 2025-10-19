using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gizmo : MonoBehaviour
{
    [SerializeField] protected List<GizmoHandle> GizmoHandles;

    public IReadOnlyList<GizmoHandle> Handles => GizmoHandles;
    private Transform m_Target;

    public Transform Target
    {
        get => m_Target;
        set
        {
            m_Target = value;
            foreach (GizmoHandle resizeHandle in GizmoHandles)
            {
                resizeHandle.transform.localScale = m_Target.localScale;
            }
        }
    }

    public GizmoType Type => GizmoType.Scale;
    

    private void Update()
    {
        if (Target)
        {
            transform.position = Target.position;
            transform.rotation = Target.rotation;
        }
    }
}