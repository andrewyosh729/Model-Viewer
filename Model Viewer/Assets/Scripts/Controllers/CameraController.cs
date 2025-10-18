using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        private CameraControls Controls { get; set; }
        private bool IsHoldingRightClick { get; set; }
        private bool IsHoldingMiddleClick { get; set; }
        private Vector3 Pivot { get; set; }
        private Vector2 AngularVelocity { get; set; }
        private Vector2 DampVelocity = Vector2.zero;
        [SerializeField] private float VelocityFalloff = 3f;
        [SerializeField] private float PanSpeed = 0.01f;
        [SerializeField] private float ZoomSpeed = 1f;


        private void Awake()
        {
            Controls = new CameraControls();
            Controls.Orbit.RightClick.started += RightClickStarted;
            Controls.Orbit.RightClick.canceled += RightClickCanceled;

            Controls.Pan.MiddleClick.started += MiddleClickStarted;
            Controls.Pan.MiddleClick.canceled += MiddleClickCanceled;
        }


        private void MiddleClickStarted(InputAction.CallbackContext obj)
        {
            IsHoldingMiddleClick = true;
        }

        private void MiddleClickCanceled(InputAction.CallbackContext obj)
        {
            IsHoldingMiddleClick = false;
        }

        private void RightClickStarted(InputAction.CallbackContext _)
        {
            IsHoldingRightClick = true;
            Pivot = Vector3.zero;
        }

        private void RightClickCanceled(InputAction.CallbackContext _)
        {
            IsHoldingRightClick = false;
            AngularVelocity = Controls.Look.MouseDelta.ReadValue<Vector2>();
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
            if (IsHoldingRightClick) // Right click takes precedence
            {
                Vector2 rotationInput = Controls.Look.MouseDelta.ReadValue<Vector2>();
                RotateAroundPivot(rotationInput);
            }
            else if (IsHoldingMiddleClick)
            {
                Vector2 panInput = Controls.Look.MouseDelta.ReadValue<Vector2>();
                transform.position += transform.right * (-panInput.x * PanSpeed);
                transform.position += transform.up * (-panInput.y * PanSpeed);
            }
            else if (AngularVelocity.magnitude > 0)
            {
                RotateAroundPivot(AngularVelocity);
                AngularVelocity =
                    Vector2.SmoothDamp(AngularVelocity, Vector2.zero, ref DampVelocity, 1f / VelocityFalloff);
            }

            if (Controls.Zoom.Scroll.inProgress)
            {
                transform.position += transform.forward * Controls.Zoom.Scroll.ReadValue<Vector2>().y * ZoomSpeed;
            }

            if (Controls.Recenter.Space.inProgress)
            {
                AngularVelocity = Vector2.zero;
                transform.position = new Vector3(0f, 0f, -10f);
                transform.rotation = Quaternion.identity;
            }
        }

        private void RotateAroundPivot(Vector2 amount)
        {
            Vector3 offset = Pivot - transform.position;
            Quaternion rotation = Quaternion.AngleAxis(amount.x, Vector3.up) *
                                  Quaternion.AngleAxis(-amount.y, transform.right);
            Vector3 rotatedOffset = rotation * offset;
            transform.position = Pivot - rotatedOffset;
            transform.rotation = rotation * transform.rotation;
        }
    }
}