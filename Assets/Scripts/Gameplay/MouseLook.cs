using UnityEngine;

namespace Gameplay
{

    public class MouseLook : MonoBehaviour
    {
        public float sensitivity = 200f;
        public Transform cam;

        private float xRotation = 0f;
        
        private bool _isActive = true;
        
        public static MouseLook Instance { get; private set; }
        
        private void Awake()
        {
            sensitivity = PlayerPrefs.GetFloat("settings.mouseSensitivity", 200f);
            Instance = this;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        public void SetActive(bool isActive)
        {
            _isActive = isActive;
            Cursor.lockState = isActive ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isActive;
        }

        private void Update()
        {
            if (!_isActive) return;
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            // Поворот камеры вверх/вниз — только по X
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -85f, 85f);
            cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // Поворот игрока (корпуса) — по Y
            transform.Rotate(Vector3.up * mouseX);
        }
    }

}