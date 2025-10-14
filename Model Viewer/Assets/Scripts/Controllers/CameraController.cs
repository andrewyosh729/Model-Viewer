using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        private CameraControls Controls { get; set; }

        private bool IsHoldingRightClick { get; set; }
        private Vector3 Pivot { get; set; }

        private void Awake()
        {
            Controls = new CameraControls();
            Controls.Orbit.RightClick.started += RightClickStarted;
            Controls.Orbit.RightClick.canceled += RightClickCanceled;
        }

        private void RightClickStarted(InputAction.CallbackContext _)
        {
            IsHoldingRightClick = true;
            Pivot = transform.position + transform.forward * 10;
        }

        private void RightClickCanceled(InputAction.CallbackContext _)
        {
            IsHoldingRightClick = false;
        }

        private void OnEnable()
        {
            Controls.Enable();
        }

        private void OnDisable()
        {
            Controls.Disable();
        }

        private void Update()
        {
            if (IsHoldingRightClick)
            {
                RotateAroundPivot();
            }
        }

        private void RotateAroundPivot()
        {
            Vector2 rotationInput = Controls.Look.MouseDelta.ReadValue<Vector2>();
            Vector3 offset = Pivot - transform.position;
            Quaternion rotation = Quaternion.AngleAxis(rotationInput.x, Vector3.up) *
                                  Quaternion.AngleAxis(-rotationInput.y, transform.right);
            Vector3 rotatedOffset = rotation * offset;
            transform.position = Pivot - rotatedOffset;
            transform.rotation = rotation * transform.rotation;
        }
    }
}