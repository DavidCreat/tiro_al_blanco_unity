using UnityEngine;
using UnityEngine.InputSystem;
using TiroAlBlanco.Core;

namespace TiroAlBlanco.Gameplay
{
    public class MouseLook : MonoBehaviour
    {
        [Header("Sensitivity")]
        [Range(0.01f, 1f)] [SerializeField] private float mouseSensitivity = 0.25f;
        [Range(10f, 90f)] [SerializeField] private float verticalClamp = 80f;

        [Header("Smoothing")]
        [SerializeField] private bool smoothLook = false;
        [Range(1f, 30f)] [SerializeField] private float smoothSpeed = 15f;

        private float xRotation;
        private float currentXRot;
        private float currentYRot;
        private bool isEnabled;

        private void Start()
        {
            xRotation = 0f;
            currentXRot = 0f;
            currentYRot = transform.parent != null ? transform.parent.eulerAngles.y : 0f;
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        private void OnEnable()
        {
            GameManager.OnGameStart += EnableLook;
            GameManager.OnGameOver += DisableLook;
            GameManager.OnGamePause += DisableLook;
            GameManager.OnGameResume += EnableLook;
        }

        private void OnDisable()
        {
            GameManager.OnGameStart -= EnableLook;
            GameManager.OnGameOver -= DisableLook;
            GameManager.OnGamePause -= DisableLook;
            GameManager.OnGameResume -= EnableLook;
        }

        private void Update()
        {
            if (!isEnabled || Mouse.current == null) return;

            Vector2 delta = Mouse.current.delta.ReadValue();
            float mouseX = delta.x * mouseSensitivity;
            float mouseY = delta.y * mouseSensitivity;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);

            if (smoothLook)
            {
                currentXRot = Mathf.Lerp(currentXRot, xRotation, smoothSpeed * Time.deltaTime);
                transform.localRotation = Quaternion.Euler(currentXRot, 0f, 0f);

                if (transform.parent != null)
                {
                    currentYRot += mouseX;
                    float smoothY = Mathf.LerpAngle(transform.parent.eulerAngles.y, currentYRot, smoothSpeed * Time.deltaTime);
                    transform.parent.rotation = Quaternion.Euler(0f, smoothY, 0f);
                }
            }
            else
            {
                transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                if (transform.parent != null)
                    transform.parent.Rotate(Vector3.up * mouseX, Space.World);
            }
        }

        public void SetSensitivity(float value) => mouseSensitivity = Mathf.Clamp(value, 0.01f, 1f);

        private void EnableLook() => isEnabled = true;
        private void DisableLook() => isEnabled = false;
    }
}
