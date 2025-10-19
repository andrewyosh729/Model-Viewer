using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using VContainer;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        public bool IsHoldingRightClick => InputService.CameraControls.Orbit.RightClick.inProgress;
        public bool IsHoldingMiddleClick => InputService.CameraControls.Pan.MiddleClick.inProgress;
        public bool IsHoldingLeftClick => InputService.CameraControls.Select.MouseClick.inProgress;
        private Vector3 Pivot { get; set; }
        private Vector2 AngularVelocity { get; set; }
        private Vector2 DampVelocity = Vector2.zero;
        [SerializeField] private float VelocityFalloff = 3f;
        [SerializeField] private float PanSpeed = 0.01f;
        [SerializeField] private float ZoomSpeed = 1f;
        [Inject] private InputService InputService { get; set; }

        private void Start()
        {
            InputService.CameraControls.Orbit.RightClick.started += RightClickStarted;
            InputService.CameraControls.Orbit.RightClick.canceled += RightClickCanceled;
        }



        private void RightClickStarted(InputAction.CallbackContext _)
        {
            Pivot = Vector3.zero;
        }

        private void RightClickCanceled(InputAction.CallbackContext _)
        {
            AngularVelocity = InputService.CameraControls.Look.MouseDelta.ReadValue<Vector2>();
        }

        private void Update()
        {
            if (IsHoldingRightClick) // Right click takes precedence
            {
                Vector2 rotationInput = InputService.CameraControls.Look.MouseDelta.ReadValue<Vector2>();
                RotateAroundPivot(rotationInput);
            }
            else if (IsHoldingMiddleClick)
            {
                Vector2 panInput = InputService.CameraControls.Look.MouseDelta.ReadValue<Vector2>();
                transform.position += transform.right * (-panInput.x * PanSpeed);
                transform.position += transform.up * (-panInput.y * PanSpeed);
            }
            else if (AngularVelocity.magnitude > 0)
            {
                RotateAroundPivot(AngularVelocity);
                AngularVelocity =
                    Vector2.SmoothDamp(AngularVelocity, Vector2.zero, ref DampVelocity, 1f / VelocityFalloff);
            }

            if (InputService.CameraControls.Zoom.Scroll.inProgress)
            {
                transform.position += transform.forward * InputService.CameraControls.Zoom.Scroll.ReadValue<Vector2>().y * ZoomSpeed;
            }

            if (InputService.CameraControls.Recenter.Space.inProgress)
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